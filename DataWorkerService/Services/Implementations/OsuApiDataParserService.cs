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
    public async Task ParseMatchAsync(Match match, MultiplayerMatch apiMatch)
    {
        logger.LogDebug("Parsing match [Id: {Id} | osu! Id: {OsuId}]", match.Id, match.OsuId);

        // Start with static match data
        match.OsuId = apiMatch.Match.Id;
        match.Name = apiMatch.Match.Name;
        match.StartTime = apiMatch.Match.StartTime.DateTime;

        // Parse games
        IEnumerable<Game> newGames = await ParseGamesAsync(apiMatch);
        foreach (Game newGame in newGames)
        {
            match.Games.Add(newGame);
        }

        match.EndTime = DetermineMatchEndTime(apiMatch) ?? default;

        logger.LogDebug("Finished parsing match [Id: {Id} | osu! Id: {OsuId}]", match.Id, match.OsuId);
    }

    public async Task<IEnumerable<Game>> ParseGamesAsync(MultiplayerMatch apiMatch)
    {
        var newGames = new List<Game>();

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

            // Try to edit an existing game
            Game? game = await gamesRepository.GetAsync(gameEvent.Game.Id);

            // Create a new game
            if (game is null)
            {
                game = new Game { OsuId = gameEvent.Game.Id };
                newGames.Add(game);

                logger.LogDebug(
                    "Created a new game [Match osu! Id: {MOsuId} | Event Id: {EvId} | Game osu! Id: {GOsuId}]",
                    apiMatch.Match.Id,
                    gameEvent.Id,
                    gameEvent.Game.Id
                );
            }
            else
            {
                logger.LogDebug(
                    "Parsing osu! API data into an existing game [Match Id: {MId} | Match osu! Id: {MOsuId} | Game Id: {GId} | Game osu! Id: {GOsuId}]",
                    game.Match.Id,
                    game.Match.OsuId,
                    game.Id,
                    game.OsuId
                );
            }

            // Set static game data
            game.Ruleset = gameEvent.Game.Ruleset;
            game.ScoringType = gameEvent.Game.ScoringType;
            game.TeamType = gameEvent.Game.TeamType;
            game.Mods = gameEvent.Game.Mods;
            game.PostModSr = gameEvent.Game.Beatmap?.StarRating ?? default;

            game.StartTime = gameEvent.Game.StartTime;

            // Determine beatmap or create a new one
            game.Beatmap ??= await beatmapsRepository.GetAsync(gameEvent.Game.BeatmapId)
                             ?? await CreateBeatmapAsync(gameEvent.Game.BeatmapId, gameEvent.Game.Beatmap);

            // Create scores
            game.Scores = (await CreateScoresAsync(gameEvent.Game.Scores, apiMatch.Users)).ToList();

            // Scale up scores set with EZ
            foreach (GameScore score in game.Scores)
            {
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
        }

        gamesRepository.AddRange(newGames);
        return newGames;
    }

    public async Task<IEnumerable<GameScore>> CreateScoresAsync(
        IEnumerable<ApiGameScore> apiScores,
        IList<MatchUser> apiPlayers
    )
    {
        var scores = new List<GameScore>();

        foreach (ApiGameScore apiScore in apiScores)
        {
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
                // Determine player
                Player = await playersRepository.GetByOsuIdAsync(apiScore.UserId)
                         ?? CreatePlayer(apiScore.UserId, apiPlayers.FirstOrDefault(p => p.Id == apiScore.UserId))
            };

            scores.Add(score);
        }

        gameScoresRepository.AddRange(scores);
        return scores;
    }

    public async Task<Beatmap> CreateBeatmapAsync(long osuBeatmapId, ApiBeatmap? apiBeatmap)
    {
        var beatmap = new Beatmap { OsuId = osuBeatmapId };

        // Try to fetch full beatmap data
        BeatmapExtended? fullApiBeatmap = await osuClient.GetBeatmapAsync(osuBeatmapId);
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
        return beatmap;
    }

    public Player CreatePlayer(long osuPlayerId, MatchUser? apiPlayer)
    {
        var player = new Player
        {
            OsuId = osuPlayerId,
            Username = apiPlayer?.Username ?? string.Empty,
            Country = apiPlayer?.CountryCode ?? string.Empty
        };

        playersRepository.Add(player);
        return player;
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
        beatmap.MaxCombo = fullApiBeatmap.MaxCombo;

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
