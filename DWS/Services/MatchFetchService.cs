using Common.Enums;
using Database;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Multiplayer;
using OsuApiClient.Enums;
using ApiGameScore = OsuApiClient.Domain.Osu.Multiplayer.GameScore;
using DbGameScore = Database.Entities.GameScore;

namespace DWS.Services;

public class MatchFetchService(
    ILogger<MatchFetchService> logger,
    OtrContext context,
    IMatchesRepository matchesRepository,
    IGamesRepository gamesRepository,
    IGameScoresRepository gameScoresRepository,
    IPlayersRepository playersRepository,
    IBeatmapsRepository beatmapsRepository,
    IOsuClient osuClient,
    IPublishEndpoint publishEndpoint)
    : IMatchFetchService
{
    public async Task<bool> FetchAndPersistMatchAsync(long osuMatchId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching match {MatchId} from osu! API", osuMatchId);

        try
        {
            // Fetch the match from osu! API
            MultiplayerMatch? apiMatch = await osuClient.GetMatchAsync(osuMatchId, cancellationToken);

            if (apiMatch is null)
            {
                logger.LogWarning("Match {MatchId} not found in osu! API", osuMatchId);

                // Check if we have existing data for this match
                IEnumerable<Match> existingMatches = await matchesRepository.GetAsync([osuMatchId]);

                if (existingMatches.Any())
                {
                    // Preserve existing data but mark as no longer available from API
                    logger.LogDebug("Match {MatchId} already exists in database, preserving existing data", osuMatchId);
                    // Note: We don't modify the existing match data as per requirements
                }

                return false;
            }

            // Process the match data
            await ProcessMatchAsync(apiMatch, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing match {MatchId}", osuMatchId);
            throw;
        }
    }

    private async Task ProcessMatchAsync(MultiplayerMatch apiMatch, CancellationToken cancellationToken)
    {
        // Check if match already exists
        IEnumerable<Match> existingMatches = await matchesRepository.GetAsync([apiMatch.Match.Id]);
        Match? existingMatch = existingMatches.FirstOrDefault();

        Match match;
        if (existingMatch is not null)
        {
            // Update existing match with fresh data
            match = existingMatch;
            UpdateMatchFromApi(match, apiMatch);
            await matchesRepository.UpdateAsync(match);
        }
        else
        {
            // Create new match
            match = CreateMatchFromApi(apiMatch);
            await matchesRepository.CreateAsync(match);
        }

        // Process players first (needed for game scores)
        await ProcessPlayersAsync(apiMatch);

        // Process games
        await ProcessGamesAsync(match, apiMatch, cancellationToken);
    }

    private static Match CreateMatchFromApi(MultiplayerMatch apiMatch)
    {
        return new Match
        {
            OsuId = apiMatch.Match.Id,
            Name = apiMatch.Match.Name,
            StartTime = DateTime.SpecifyKind(apiMatch.Match.StartTime, DateTimeKind.Utc),
            EndTime = apiMatch.Match.EndTime.HasValue
                ? DateTime.SpecifyKind(apiMatch.Match.EndTime.Value, DateTimeKind.Utc)
                : null
        };
    }

    private static void UpdateMatchFromApi(Match match, MultiplayerMatch apiMatch)
    {
        match.Name = apiMatch.Match.Name;
        match.StartTime = DateTime.SpecifyKind(apiMatch.Match.StartTime, DateTimeKind.Utc);
        match.EndTime = apiMatch.Match.EndTime.HasValue
            ? DateTime.SpecifyKind(apiMatch.Match.EndTime.Value, DateTimeKind.Utc)
            : null;
    }

    private async Task ProcessPlayersAsync(MultiplayerMatch apiMatch)
    {
        // Get all unique player IDs from match users
        var playerIds = apiMatch.Users
            .Select(u => u.Id)
            .Distinct()
            .ToList();

        // Check which players already exist
        IEnumerable<Player> existingPlayers = await playersRepository.GetAsync(playerIds);
        var existingPlayerIds = existingPlayers.Select(p => p.OsuId).ToHashSet();

        // Create missing players
        var newPlayers = apiMatch.Users
            .Where(u => !existingPlayerIds.Contains(u.Id))
            .Select(u => new Player
            {
                OsuId = u.Id,
                Username = u.Username,
                Country = u.CountryCode
            })
            .DistinctBy(p => p.OsuId)
            .ToList();

        if (newPlayers.Count != 0)
        {
            foreach (Player player in newPlayers)
            {
                await playersRepository.CreateAsync(player);
            }
        }
    }

    private async Task ProcessGamesAsync(Match match, MultiplayerMatch apiMatch, CancellationToken cancellationToken)
    {
        // Extract games from match events
        var gameEvents = apiMatch.Events
            .Where(e => e.Detail.Type == MultiplayerEventType.Game && e.Game != null)
            .Select(e => e.Game!)
            .ToList();

        foreach (MultiplayerGame apiGame in gameEvents)
        {
            // Check if game already exists
            // Note: We search by osu ID as games should be unique by their osu ID
            Game? existingGame = await context.Games
                .Where(g => g.OsuId == apiGame.Id)
                .FirstOrDefaultAsync(cancellationToken);

            Game game;
            if (existingGame is not null)
            {
                // Update existing game
                game = existingGame;
                UpdateGameFromApi(game, apiGame);
                await gamesRepository.UpdateAsync(game);
            }
            else
            {
                // Create new game
                game = await CreateGameFromApiAsync(match, apiGame, cancellationToken);
                await gamesRepository.CreateAsync(game);
            }

            // Process game scores
            await ProcessGameScoresAsync(game, apiGame, cancellationToken);
        }
    }

    private async Task<Game> CreateGameFromApiAsync(Match match, MultiplayerGame apiGame, CancellationToken cancellationToken)
    {
        Beatmap? beatmap = null;

        // Ensure beatmap exists
        if (apiGame.BeatmapId > 0)
        {
            beatmap = await beatmapsRepository.GetAsync(apiGame.BeatmapId);
            if (beatmap is null)
            {
                // Create placeholder beatmap
                beatmap = new Beatmap
                {
                    OsuId = apiGame.BeatmapId
                };
                await beatmapsRepository.CreateAsync(beatmap);

                // Save changes to persist the beatmap before creating the game
                await context.SaveChangesAsync(cancellationToken);

                // Queue it for processing
                var fetchBeatmapMessage = new FetchBeatmapMessage
                {
                    BeatmapId = apiGame.BeatmapId,
                    Priority = MessagePriority.Normal
                };

                await publishEndpoint.Publish(fetchBeatmapMessage, ctx =>
                {
                    ctx.SetPriority((byte)fetchBeatmapMessage.Priority);
                }, cancellationToken);

                logger.LogDebug("Queued beatmap {BeatmapId} for fetching", apiGame.BeatmapId);
            }
        }

        return new Game
        {
            OsuId = apiGame.Id,
            MatchId = match.Id,
            BeatmapId = beatmap?.Id,
            StartTime = DateTime.SpecifyKind(apiGame.StartTime, DateTimeKind.Utc),
            EndTime = DateTime.SpecifyKind(apiGame.EndTime ?? apiGame.StartTime, DateTimeKind.Utc),
            Ruleset = apiGame.Ruleset,
            ScoringType = apiGame.ScoringType,
            TeamType = apiGame.TeamType,
            Mods = apiGame.Mods
        };
    }

    private static void UpdateGameFromApi(Game game, MultiplayerGame apiGame)
    {
        game.StartTime = DateTime.SpecifyKind(apiGame.StartTime, DateTimeKind.Utc);
        game.EndTime = DateTime.SpecifyKind(apiGame.EndTime ?? game.StartTime, DateTimeKind.Utc);
        game.Ruleset = apiGame.Ruleset;
        game.ScoringType = apiGame.ScoringType;
        game.TeamType = apiGame.TeamType;
        game.Mods = apiGame.Mods;
    }

    private async Task ProcessGameScoresAsync(Game game, MultiplayerGame apiGame, CancellationToken cancellationToken)
    {
        // Get existing scores for this game
        List<DbGameScore> existingScores = await context.GameScores
            .Include(gs => gs.Player)
            .Where(gs => gs.GameId == game.Id)
            .ToListAsync(cancellationToken);
        var existingScoreMap = new Dictionary<(int, Team), DbGameScore>();

        foreach (DbGameScore score in existingScores)
        {
            existingScoreMap[(score.PlayerId, score.Team)] = score;
        }

        foreach (ApiGameScore apiScore in apiGame.Scores)
        {
            IEnumerable<Player> players = await playersRepository.GetAsync([apiScore.UserId]);
            Player? player = players.FirstOrDefault();

            if (player is null)
            {
                logger.LogWarning("Player {PlayerId} not found for score in game {GameId}", apiScore.UserId, game.Id);
                continue;
            }

            (int Id, Team Team) key = (player.Id, apiScore.SlotInfo.Team);

            if (existingScoreMap.TryGetValue(key, out DbGameScore? existingScore))
            {
                // Update existing score
                UpdateScoreFromApi(existingScore, apiScore);
                await gameScoresRepository.UpdateAsync(existingScore);
            }
            else
            {
                // Create new score
                DbGameScore score = CreateScoreFromApi(game, player, apiScore);
                await gameScoresRepository.CreateAsync(score);
            }
        }
    }

    private static DbGameScore CreateScoreFromApi(Game game, Player player, ApiGameScore apiScore)
    {
        return new DbGameScore
        {
            GameId = game.Id,
            PlayerId = player.Id,
            Team = apiScore.SlotInfo.Team,
            Score = apiScore.Score,
            MaxCombo = apiScore.MaxCombo,
            Count300 = apiScore.Statistics?.Count300 ?? 0,
            Count100 = apiScore.Statistics?.Count100 ?? 0,
            Count50 = apiScore.Statistics?.Count50 ?? 0,
            CountMiss = apiScore.Statistics?.CountMiss ?? 0,
            CountGeki = apiScore.Statistics?.CountGeki ?? 0,
            CountKatu = apiScore.Statistics?.CountKatu ?? 0,
            Pass = apiScore.Passed,
            Mods = apiScore.Mods,
            Ruleset = game.Ruleset
        };
    }

    private static void UpdateScoreFromApi(DbGameScore score, ApiGameScore apiScore)
    {
        score.Score = apiScore.Score;
        score.MaxCombo = apiScore.MaxCombo;
        score.Count300 = apiScore.Statistics?.Count300 ?? 0;
        score.Count100 = apiScore.Statistics?.Count100 ?? 0;
        score.Count50 = apiScore.Statistics?.Count50 ?? 0;
        score.CountMiss = apiScore.Statistics?.CountMiss ?? 0;
        score.CountGeki = apiScore.Statistics?.CountGeki ?? 0;
        score.CountKatu = apiScore.Statistics?.CountKatu ?? 0;
        score.Pass = apiScore.Passed;
        score.Mods = apiScore.Mods;
    }
}
