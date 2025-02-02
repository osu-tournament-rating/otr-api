using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Enums;
using Database.Repositories.Interfaces;
using DataWorkerService.Services.Interfaces;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Beatmaps;
using OsuApiClient.Domain.Osu.Multiplayer;
using OsuApiClient.Enums;
using ApiBeatmap = OsuApiClient.Domain.Osu.Beatmaps.Beatmap;
using ApiGameScore = OsuApiClient.Domain.Osu.Multiplayer.GameScore;
using Beatmap = Database.Entities.Beatmap;
using GameScore = Database.Entities.GameScore;
using User = OsuApiClient.Domain.Osu.Users.User;

namespace DataWorkerService.Services.Implementations;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class OsuApiDataParserService(
    ILogger<OsuApiDataParserService> logger,
    IGamesRepository gamesRepository,
    IBeatmapsRepository beatmapsRepository,
    IPlayersRepository playersRepository,
    IGameScoresRepository gameScoresRepository,
    IOsuClient osuClient
) : IOsuApiDataParserService
{
    private readonly Dictionary<long, Player> _playersCache = [];
    private readonly Dictionary<long, Beatmap> _beatmapsCache = [];

    public async Task ParseMatchAsync(Match match, MultiplayerMatch apiMatch)
    {
        logger.LogDebug("Parsing match [Id: {Id} | osu! Id: {OsuId}]", match.Id, match.OsuId);

        // Start with static match data
        match.OsuId = apiMatch.Match.Id;
        match.Name = apiMatch.Match.Name;
        match.StartTime = apiMatch.Match.StartTime.DateTime;

        // Load players and beatmaps
        await LoadPlayersAsync(apiMatch.Users);
        await LoadBeatmapsAsync(apiMatch.Events.Select(e => e.Game).OfType<MultiplayerGame>());

        // Parse games
        match.Games = CreateGames(apiMatch).ToList();

        match.EndTime = DetermineMatchEndTime(apiMatch) ?? default;

        logger.LogDebug("Finished parsing match [Id: {Id} | osu! Id: {OsuId}]", match.Id, match.OsuId);
    }

    public async Task LoadPlayersAsync(IEnumerable<User> apiPlayers)
    {
        apiPlayers = apiPlayers.ToList();
        var uncachedPlayerOsuIds = apiPlayers
            .Select(p => p.Id)
            .Distinct()
            .Where(osuId => !_playersCache.ContainsKey(osuId))
            .ToList();

        if (uncachedPlayerOsuIds.Count == 0)
        {
            return;
        }

        IEnumerable<Player> fetchedPlayers = (await playersRepository.GetByOsuIdAsync(uncachedPlayerOsuIds)).ToList();

        foreach (var playerOsuId in uncachedPlayerOsuIds)
        {
            Player? player = fetchedPlayers.FirstOrDefault(p => p.OsuId == playerOsuId);
            if (player is null)
            {
                User? apiPlayer = apiPlayers.FirstOrDefault(p => p.Id == playerOsuId);

                player = new Player
                {
                    OsuId = playerOsuId,
                    Username = apiPlayer?.Username ?? string.Empty,
                    Country = apiPlayer?.CountryCode ?? string.Empty
                };

                playersRepository.Add(player);
            }

            _playersCache.Add(playerOsuId, player);
        }
    }

    public async Task LoadBeatmapsAsync(IEnumerable<MultiplayerGame> apiGames)
    {
        apiGames = apiGames.ToList();

        var uncachedBeatmapOsuIds = apiGames
            .Select(g => g.BeatmapId)
            .Distinct()
            .Where(osuId => !_beatmapsCache.ContainsKey(osuId))
            .ToList();

        /**
         * Sync the cache by pulling existing beatmap entities or creating new entities as needed.
         * This step ensures that each unique beatmap id we encounter has an entity created for it.
         */
        if (uncachedBeatmapOsuIds.Count != 0)
        {
            IEnumerable<Beatmap> fetchedBeatmaps = (await beatmapsRepository.GetAsync(uncachedBeatmapOsuIds)).ToList();

            foreach (var beatmapOsuId in uncachedBeatmapOsuIds)
            {
                Beatmap? beatmap = fetchedBeatmaps.FirstOrDefault(b => b.OsuId == beatmapOsuId);

                if (beatmap is null)
                {
                    beatmap = new Beatmap { OsuId = beatmapOsuId };
                    beatmapsRepository.Add(beatmap);
                }

                _beatmapsCache.Add(beatmap.OsuId, beatmap);
            }
        }

        /**
         * By filtering for only non-null api beatmap objects using `.OfType<ApiBeatmap>()`, we can
         * efficiently back-fill the cached beatmap entities with API data. This will populate data for
         * newly created beatmaps, beatmaps created from submission, and even potentially fill in
         * missing data from beatmaps that were not parsed properly.
         */
        foreach (ApiBeatmap apiBeatmap in apiGames.Select(g => g.Beatmap).OfType<ApiBeatmap>())
        {
            if (!_beatmapsCache.TryGetValue(apiBeatmap.Id, out Beatmap? cachedBeatmap))
            {
                logger.LogCritical("Beatmap should never be null here");
                throw new Exception("Beatmap cache miss during beatmap loading");
            }

            // Skip unnecessary API call if we already have data for this beatmap
            if (cachedBeatmap.HasData)
            {
                continue;
            }

            // Try to fetch the full beatmap first, fallback by using partial data from the game event
            BeatmapExtended? fullApiBeatmap = await osuClient.GetBeatmapAsync(apiBeatmap.Id);

            if (fullApiBeatmap is not null)
            {
                await ParseBeatmap(cachedBeatmap, fullApiBeatmap);
            }
            else
            {
                await ParseBeatmapPartial(cachedBeatmap, apiBeatmap);
            }
        }
    }

    public IEnumerable<Game> CreateGames(MultiplayerMatch apiMatch)
    {
        var games = new List<Game>();

        foreach (MatchEvent gameEvent in apiMatch.Events.Where(ev => ev.Detail.Type == MultiplayerEventType.Game))
        {
            logger.LogDebug(
                "Parsing game [Match osu! Id: {MOsuId} | Event Id: {EvId}]",
                apiMatch.Match.Id,
                gameEvent.Id
            );

            // Sanity check
            if (gameEvent.Game is null)
            {
                // Note: This could be filtered in the LINQ statement, but we want to know when this happens
                logger.LogError(
                    "Match event type was 'Game' but a game was not included, skipping [Match osu! Id: {OsuId} | Event Id: {EvId}]",
                    apiMatch.Match.Id,
                    gameEvent.Id
                );

                continue;
            }

            if (gameEvent.Game.Scores.Length == 0)
            {
                logger.LogDebug(
                    "Game contains no scores and was likely aborted, skipping [Match osu! Id: {OsuId} | Event Id: {EvId}]",
                    apiMatch.Match.Id,
                    gameEvent.Id
                );

                continue;
            }

            var game = new Game
            {
                OsuId = gameEvent.Game.Id,
                Ruleset = gameEvent.Game.Ruleset,
                ScoringType = gameEvent.Game.ScoringType,
                TeamType = gameEvent.Game.TeamType,
                Mods = gameEvent.Game.Mods,
                StartTime = gameEvent.Game.StartTime,
                Beatmap = _beatmapsCache.FirstOrDefault(b => b.OsuId == gameEvent.Game.BeatmapId),
                Scores = CreateScores(gameEvent.Game.Scores).ToList()
            };

            foreach (GameScore score in game.Scores)
            {
                // Scale up scores set with EZ
                if (game.Mods.HasFlag(Mods.Easy) || score.Mods.HasFlag(Mods.Easy))
                {
                    score.Score = (int)(score.Score * 1.75);
                }
            }

            // Determine end time
            DateTime? endTime = DetermineGameEndTime(game, gameEvent.Game);
            if (endTime.HasValue)
            {
                game.EndTime = endTime.Value;
            }

            logger.LogDebug(
                "Finished parsing game [Match osu! Id: {MOsuId} | Event Id: {EvId}]",
                apiMatch.Match.Id,
                gameEvent.Id
            );

            games.Add(game);
        }

        gamesRepository.AddRange(games);
        return games;
    }

    public IEnumerable<GameScore> CreateScores(IEnumerable<ApiGameScore> apiScores)
    {
        var scores = new List<GameScore>();

        foreach (ApiGameScore apiScore in apiScores)
        {
            if (!_playersCache.TryGetValue(apiScore.UserId, out Player? player))
            {
                logger.LogError("Expected player to be loaded, skipping score [Player osu! id: {OsuId}]", apiScore.UserId);
                continue;
            }

            var score = new GameScore
            {
                Score = apiScore.Score,
                MaxCombo = apiScore.MaxCombo,
                Count50 = apiScore.Statistics.Count50,
                Count100 = apiScore.Statistics.Count100,
                Count300 = apiScore.Statistics.Count300,
                CountMiss = apiScore.Statistics.CountMiss,
                CountGeki = apiScore.Statistics.CountGeki,
                CountKatu = apiScore.Statistics.CountKatu,
                Pass = apiScore.Passed,
                Perfect = apiScore.Perfect != 0,
                Grade = apiScore.Grade,
                Mods = apiScore.Mods,
                Ruleset = apiScore.Ruleset,
                Team = apiScore.SlotInfo.Team,
                Player = player
            };

            scores.Add(score);
        }

        gameScoresRepository.AddRange(scores);
        return scores;
    }

    public Task ParseBeatmapPartial(Beatmap beatmap, ApiBeatmap apiBeatmap)
    {
        beatmap.OsuId = apiBeatmap.Id;
        beatmap.Ruleset = apiBeatmap.Ruleset;
        beatmap.TotalLength = apiBeatmap.TotalLength;
        beatmap.DiffName = apiBeatmap.DifficultyName;

        if (apiBeatmap.Beatmapset?.CreatorId is null)
        {
            return Task.CompletedTask;
        }

        if (!_playersCache.TryGetValue(apiBeatmap.Beatmapset.CreatorId.Value, out Player? creatorPlayer))
        {
            logger.LogCritical("Player should never be null here");
            throw new Exception("Player cache miss during partial beatmap parsing");
        }

        // Create or update BeatmapSet
        beatmap.BeatmapSet = new BeatmapSet
        {
            OsuId = apiBeatmap.Beatmapset.Id,
            Artist = apiBeatmap.Beatmapset.Artist,
            Title = apiBeatmap.Beatmapset.Title,
            CreatorId = creatorPlayer.Id,
            RankedStatus = apiBeatmap.Beatmapset.RankedStatus,
            SubmittedDate = apiBeatmap.Beatmapset.SubmittedDate,
            RankedDate = apiBeatmap.Beatmapset.RankedDate
        };

        return Task.CompletedTask;
    }

    public Task ParseBeatmap(Beatmap beatmap, BeatmapExtended fullApiBeatmap)
    {
        ParseBeatmapPartial(beatmap, fullApiBeatmap);

        beatmap.Sr = fullApiBeatmap.StarRating;
        beatmap.Bpm = fullApiBeatmap.Bpm;
        beatmap.Cs = fullApiBeatmap.CircleSize;
        beatmap.Ar = fullApiBeatmap.ApproachRate;
        beatmap.Hp = fullApiBeatmap.HpDrain;
        beatmap.Od = fullApiBeatmap.OverallDifficulty;
        beatmap.CountCircle = fullApiBeatmap.CountCircles;
        beatmap.CountSlider = fullApiBeatmap.CountSliders;
        beatmap.CountSpinner = fullApiBeatmap.CountSpinners;
        beatmap.MaxCombo = fullApiBeatmap.MaxCombo;

        beatmap.HasData = true;
        return Task.CompletedTask;
    }

    public static DateTime? DetermineMatchEndTime(MultiplayerMatch apiMatch)
    {
        // Return given end time if present
        if (apiMatch.Match.EndTime.HasValue)
        {
            return apiMatch.Match.EndTime.Value.DateTime;
        }

        // Try to use the timestamp of a disband event if available
        MatchEvent? disbandEvent = apiMatch.Events.FirstOrDefault(ev => ev.Detail.Type == MultiplayerEventType.MatchDisbanded);
        if (disbandEvent is not null)
        {
            return disbandEvent.Timestamp;
        }

        // Try to use the timestamp of the last game event if available
        MatchEvent? lastGame = apiMatch.Events
            .Where(ev => ev.Detail.Type == MultiplayerEventType.Game)
            .MaxBy(ev => ev.Timestamp);

        return lastGame?.Timestamp;
    }

    public static DateTime? DetermineGameEndTime(Game game, MultiplayerGame apiGame)
    {
        if (apiGame.EndTime.HasValue)
        {
            return apiGame.EndTime.Value;
        }

        // Try to add the length of the beatmap to the start time
        if (apiGame.Beatmap is not null)
        {
            return apiGame.StartTime.AddSeconds(apiGame.Beatmap.TotalLength);
        }

        if (game.Beatmap is not null)
        {
            return apiGame.StartTime.AddSeconds(game.Beatmap.TotalLength);
        }

        return null;
    }
}
