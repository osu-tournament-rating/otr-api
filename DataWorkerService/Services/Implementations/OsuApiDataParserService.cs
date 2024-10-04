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

namespace DataWorkerService.Services.Implementations;

public class OsuApiDataParserService(
    ILogger<OsuApiDataParserService> logger,
    IGamesRepository gamesRepository,
    IBeatmapsRepository beatmapsRepository,
    IPlayersRepository playersRepository,
    IGameScoresRepository gameScoresRepository,
    IOsuClient osuClient
) : IOsuApiDataParserService
{
    private readonly List<Player> _playersCache = [];
    private readonly List<Beatmap> _beatmapsCache = [];

    public async Task ParseMatchAsync(Match match, MultiplayerMatch apiMatch)
    {
        logger.LogDebug("Parsing match [Id: {Id} | osu! Id: {OsuId}]", match.Id, match.OsuId);

        // Start with static match data
        match.OsuId = apiMatch.Match.Id;
        match.Name = apiMatch.Match.Name;
        match.StartTime = apiMatch.Match.StartTime.DateTime;

        // Load players and beatmaps
        await LoadPlayersAsync(apiMatch.Users);
        await LoadBeatmapsAsync(apiMatch.Events.Where(e => e.Game != null).Select(e => e.Game));

        // Parse games
        match.Games = CreateGames(apiMatch).ToList();

        match.EndTime = DetermineMatchEndTime(apiMatch) ?? default;

        logger.LogDebug("Finished parsing match [Id: {Id} | osu! Id: {OsuId}]", match.Id, match.OsuId);
    }

    public async Task LoadPlayersAsync(IEnumerable<MatchUser> apiPlayers)
    {
        apiPlayers = apiPlayers.ToList();
        var uncachedPlayerOsuIds = apiPlayers
            .Select(p => p.Id)
            .Distinct()
            .Where(osuId => !_playersCache.Select(p => p.OsuId).Contains(osuId))
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
                MatchUser? apiPlayer = apiPlayers.FirstOrDefault(p => p.Id == playerOsuId);

                player = new Player
                {
                    OsuId = playerOsuId,
                    Username = apiPlayer?.Username ?? string.Empty,
                    Country = apiPlayer?.CountryCode ?? string.Empty
                };

                playersRepository.Add(player);
            }

            _playersCache.Add(player);
        }
    }

    public async Task LoadBeatmapsAsync(IEnumerable<MultiplayerGame?> apiGames)
    {
        apiGames = apiGames.ToList();

        var uncachedBeatmapOsuIds = apiGames
            .Select(g => g!.BeatmapId)
            .Distinct()
            .Where(osuId => !_beatmapsCache.Select(b => b.OsuId).Contains(osuId))
            .ToList();

        if (uncachedBeatmapOsuIds.Count == 0)
        {
            return;
        }

        IEnumerable<Beatmap> fetchedBeatmaps = (await beatmapsRepository.GetAsync(uncachedBeatmapOsuIds)).ToList();

        foreach (var beatmapOsuId in uncachedBeatmapOsuIds)
        {
            Beatmap? beatmap = fetchedBeatmaps.FirstOrDefault(b => b.OsuId == beatmapOsuId);

            if (beatmap is null)
            {
                beatmap = new Beatmap { OsuId = beatmapOsuId };

                ApiBeatmap? apiBeatmap = apiGames
                    .Select(g => g?.Beatmap)
                    .FirstOrDefault(b => b?.Id == beatmapOsuId);
                BeatmapExtended? fullApiBeatmap = await osuClient.GetBeatmapAsync(beatmapOsuId);
                if (fullApiBeatmap is not null)
                {
                    ParseBeatmap(beatmap, fullApiBeatmap);
                    logger.LogInformation("Created a new beatmap with full data: [Osu Id: {OsuId}]", beatmap.OsuId);
                }
                // Use given partial data if available
                else if (apiBeatmap is not null)
                {
                    ParseBeatmapPartial(beatmap, apiBeatmap);
                    logger.LogInformation("Created a new beatmap with partial data: [Osu Id: {OsuId}]", beatmap.OsuId);
                }
                else
                {
                    logger.LogInformation("Created a new beatmap with no data: [Osu Id: {OsuId}]", beatmap.OsuId);
                }

                beatmapsRepository.Add(beatmap);
            }

            _beatmapsCache.Add(beatmap);
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
            Player? player = _playersCache.FirstOrDefault(p => p.OsuId == apiScore.UserId);
            if (player is null)
            {
                logger.LogError("Player was not loaded correctly, skipping score [Player osu! id: {osuId}]", apiScore.UserId);

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

    public static void ParseBeatmapPartial(Beatmap beatmap, ApiBeatmap apiBeatmap)
    {
        beatmap.OsuId = apiBeatmap.Id;
        beatmap.Ruleset = apiBeatmap.Ruleset;
        beatmap.Length = apiBeatmap.TotalLength;
        beatmap.DiffName = apiBeatmap.DifficultyName;

        if (apiBeatmap.Beatmapset is null)
        {
            return;
        }

        beatmap.Artist = apiBeatmap.Beatmapset.Artist;
        beatmap.Title = apiBeatmap.Beatmapset.Title;
        beatmap.MapperName = apiBeatmap.Beatmapset.Creator ?? string.Empty;
        beatmap.MapperId = apiBeatmap.Beatmapset.CreatorId ?? default;
    }

    public static void ParseBeatmap(Beatmap beatmap, BeatmapExtended fullApiBeatmap)
    {
        ParseBeatmapPartial(beatmap, fullApiBeatmap);

        beatmap.Sr = fullApiBeatmap.StarRating;
        beatmap.Bpm = fullApiBeatmap.Bpm;
        beatmap.Cs = fullApiBeatmap.CircleSize;
        beatmap.Ar = fullApiBeatmap.ApproachRate;
        beatmap.Hp = fullApiBeatmap.HpDrain;
        beatmap.Od = fullApiBeatmap.OverallDifficulty;
        beatmap.RankedStatus = fullApiBeatmap.RankedStatus;
        beatmap.CircleCount = fullApiBeatmap.CountCircles;
        beatmap.SliderCount = fullApiBeatmap.CountSliders;
        beatmap.SpinnerCount = fullApiBeatmap.CountSpinners;
        beatmap.MaxCombo = fullApiBeatmap.MaxCombo ?? default;

        beatmap.HasData = true;
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
            return apiGame.StartTime.AddSeconds(game.Beatmap.Length);
        }

        return null;
    }
}
