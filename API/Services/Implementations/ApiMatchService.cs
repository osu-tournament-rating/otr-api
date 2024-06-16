using System.Diagnostics.CodeAnalysis;
using API.Osu.Multiplayer;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Database;
using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;


/// <summary>
/// Strictly responsible for processing matches from the osu! API and adding them to the database. This includes:
/// * Player data
/// * Beatmap data
/// * Match data, game data, and score data
/// </summary>
[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class ApiMatchService(
    ILogger<ApiMatchService> logger,
    OtrContext context,
    IApiPlayersRepository playerRepository,
    IBeatmapRepository beatmapRepository,
    IOsuApiService osuApiService,
    IMatchesRepository matchesRepository,
    IGamesRepository gamesRepository,
    IMatchScoresRepository matchScoresRepository
    ) : IApiMatchService
{
    public async Task<Match?> CreateFromApiMatchAsync(OsuApiMatchData apiMatch)
    {
        logger.LogInformation("Processing match {MatchId}", apiMatch.OsuApiMatch.MatchId);

        await CreatePlayersAsync(apiMatch);
        await CreateBeatmapsAsync(apiMatch);
        return await UpdateMatchDataAsync(apiMatch);
    }

    private async Task CreatePlayersAsync(OsuApiMatchData apiMatch)
    {
        Dictionary<long, int>? existingPlayersMapping = await ExistingPlayersMapping(apiMatch);

        if (existingPlayersMapping == null)
        {
            return;
        }

        foreach (var osuId in GetUserIdsFromMatch(apiMatch))
        {
            if (existingPlayersMapping.ContainsKey(osuId))
            {
                // No need to create the player, they already exist
                continue;
            }

            var newPlayer = new Player { OsuId = osuId };
            Player player = await playerRepository.CreateAsync(newPlayer);

            logger.LogInformation(
                "Saved new player: {PlayerId} (osuId: {OsuId}) from match {MatchId}",
                player.Id,
                osuId,
                apiMatch.OsuApiMatch.MatchId
            );
        }
    }

    /// <summary>
    ///  Gets all players from the database that are in the match.
    /// </summary>
    /// <param name="apiMatch"></param>
    /// <returns></returns>
    private async Task<Dictionary<long, int>?> ExistingPlayersMapping(OsuApiMatchData apiMatch)
    {
        // Select all osu! user ids from the match's scores
        List<long> osuPlayerIds = GetUserIdsFromMatch(apiMatch);

        if (osuPlayerIds.Count == 0)
        {
            logger.LogError("No players found in match {MatchId}", apiMatch.OsuApiMatch.MatchId);
            return null;
        }

        // Select all players from the database that are in the match
        List<Player> existingPlayers = await context.Players.Where(p => osuPlayerIds.Contains(p.OsuId)).ToListAsync();

        logger.LogTrace(
            "Identified {Count} existing players to add for match {MatchId}",
            existingPlayers.Count,
            apiMatch.OsuApiMatch.MatchId
        );

        // Map these osu! ids to their database ids
        return existingPlayers.ToDictionary(player => player.OsuId, player => player.Id);
    }

    private static List<long> GetUserIdsFromMatch(OsuApiMatchData apiMatch) =>
        apiMatch.Games.SelectMany(x => x.Scores).Select(x => x.UserId).Distinct().ToList();

    // Beatmaps

    /// <summary>
    ///  Saves the beatmaps identified in the match to the database. Only save complete beatmaps. This does call the osu! API.
    /// </summary>
    private async Task CreateBeatmapsAsync(OsuApiMatchData apiMatch)
    {
        var beatmapIds = GetBeatmapIds(apiMatch)?.Distinct().ToList();

        if (beatmapIds == null || beatmapIds.Count == 0)
        {
            logger.LogError("No beatmap IDs found in match {MatchId}", apiMatch.OsuApiMatch.MatchId);
            return;
        }

        var beatmapsToSave = new List<Beatmap>();
        var countSaved = 0;

        foreach (var beatmapId in beatmapIds)
        {
            Beatmap? existingBeatmap = await beatmapRepository.GetAsync(beatmapId);
            if (existingBeatmap == null)
            {
                Beatmap? beatmap = await osuApiService.GetBeatmapAsync(
                    beatmapId,
                    $"Beatmap {beatmapId} from match {apiMatch.OsuApiMatch.MatchId} does not exist in database"
                );

                if (beatmap != null)
                {
                    beatmapsToSave.Add(beatmap);
                    countSaved++;
                }
            }
        }

        await beatmapRepository.BulkInsertAsync(beatmapsToSave);

        if (countSaved > 0)
        {
            logger.LogInformation(
                "Saved {Count} beatmaps from match {MatchId}",
                countSaved,
                apiMatch.OsuApiMatch.MatchId
            );
        }
    }

    private static IEnumerable<long> GetBeatmapIds(OsuApiMatchData apiMatch) =>
        apiMatch.Games.Select(x => x.BeatmapId);

    // Match
    /// <summary>
    /// Updates an existing database match with data from the osu! API.
    /// </summary>
    /// <param name="apiMatch"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">If the existing match is null</exception>
    /// <exception cref="InvalidOperationException">If the Match.IsApiProcessed flag is null or if it is true</exception>
    private async Task<Match?> UpdateMatchDataAsync(OsuApiMatchData apiMatch)
    {
        Match? existingMatch = await ExistingMatch(apiMatch) ?? throw new NullReferenceException(
                $"Match {apiMatch.OsuApiMatch.MatchId} does not exist in database. This should not be possible as it should have been created by POST /api/matches/batch"
            );
        if (existingMatch.IsApiProcessed == null)
        {
            throw new InvalidOperationException(
                $"Match {apiMatch.OsuApiMatch.MatchId} has no IsApiProcessed value! This should not be possible as it should have been set by POST /api/matches/batch"
            );
        }

        if (existingMatch.IsApiProcessed.Value)
        {
            // The match has already been marked as api processed. This shouldn't be possible.
            throw new InvalidOperationException(
                $"Match {apiMatch.OsuApiMatch.MatchId} has already been marked as api processed! This should not be possible as it should have been set by POST /api/matches/batch"
            );
        }

        existingMatch = await UpdateMatchAsync(apiMatch, existingMatch);

        IList<Game> persistedGames = await CreateGamesAsync(apiMatch.Games, existingMatch);
        foreach (OsuApiGame game in apiMatch.Games)
        {
            await CreateScoresAsync(game);
        }

        if (persistedGames.Count > 0)
        {
            logger.LogInformation(
                "Saved scores for {Count} games from match {MatchId}",
                persistedGames.Count,
                apiMatch.OsuApiMatch.MatchId
            );
        }

        // Fetch the full entity from the database once again to ensure we have the latest populated data
        return await matchesRepository.GetAsync(existingMatch.Id);
    }

    private async Task<Match?> ExistingMatch(OsuApiMatchData apiMatch) =>
        await context.Matches.FirstOrDefaultAsync(x => x.MatchId == apiMatch.OsuApiMatch.MatchId);

    private async Task<Match> UpdateMatchAsync(OsuApiMatchData apiMatch, Match existingMatch)
    {
        existingMatch.Name = apiMatch.OsuApiMatch.Name;
        existingMatch.StartTime = apiMatch.OsuApiMatch.StartTime;
        existingMatch.EndTime = apiMatch.OsuApiMatch.EndTime!.Value;
        existingMatch.IsApiProcessed = true;

        await matchesRepository.UpdateAsync(existingMatch);

        logger.LogInformation(
            "Updated match: {MatchId} (name: {MatchName})",
            existingMatch.MatchId,
            existingMatch.Name
        );
        return existingMatch;
    }

    // Games
    /// <summary>
    /// Persists all games from the osu! API to the database.
    /// </summary>
    /// <param name="osuMatchGames"></param>
    /// <param name="existingMatch"></param>
    /// <returns>List of persisted entities</returns>
    public async Task<IList<Game>> CreateGamesAsync(
        IEnumerable<OsuApiGame> osuMatchGames,
        Match existingMatch
    )
    {
        var persisted = new List<Game>();
        foreach (OsuApiGame game in osuMatchGames)
        {
            var beatmapIdResult = await beatmapRepository.GetIdAsync(game.BeatmapId);

            Game? existingGame = await context.Games.FirstOrDefaultAsync(g => g.GameId == game.GameId);

            if (existingGame == null)
            {
                Game? newGame = await AddNewGameAsync(game, existingMatch, beatmapIdResult);

                if (newGame == null)
                {
                    // Something seriously went wrong.
                    logger.LogError("Failed to save new game {GameId}!", game.GameId);
                    continue;
                }

                persisted.Add(newGame);
            }
        }

        return persisted;
    }

    /// <summary>
    /// Persists a new game to the database
    /// </summary>
    /// <param name="osuApiGame"></param>
    /// <param name="existingMatch"></param>
    /// <param name="beatmapIdResult"></param>
    private async Task<Game?> AddNewGameAsync(
        OsuApiGame osuApiGame,
        Match existingMatch,
        int? beatmapIdResult
    )
    {
        var dbGame = new Game
        {
            MatchId = existingMatch.Id,
            GameId = osuApiGame.GameId,
            StartTime = osuApiGame.StartTime,
            EndTime = osuApiGame.EndTime!.Value,
            BeatmapId = beatmapIdResult,
            Ruleset = osuApiGame.Ruleset,
            ScoringType = osuApiGame.ScoringType,
            TeamType = osuApiGame.TeamType,
            Mods = osuApiGame.Mods
        };

        Game persisted = await gamesRepository.CreateAsync(dbGame);
        logger.LogDebug("Saved game {GameId}", dbGame.GameId);

        return persisted;
    }

    private async Task<Game?> GetGameFromDatabase(long gameId)
    {
        return await context.Games.FirstOrDefaultAsync(x => x.GameId == gameId);
    }

    // Scores
    private async Task CreateScoresAsync(OsuApiGame osuApiGame)
    {
        Game? dbGame = await GetGameFromDatabase(osuApiGame.GameId);
        if (dbGame == null)
        {
            logger.LogError(
                "Failed to fetch game {GameId} from database while processing scores! This means {Count} scores will be missing for this game!",
                osuApiGame.GameId,
                osuApiGame.Scores.Count
            );

            return;
        }

        var countSaved = 0;
        foreach (OsuApiScore score in osuApiGame.Scores)
        {
            var playerId = await playerRepository.GetIdAsync(score.UserId);
            if (!playerId.HasValue)
            {
                logger.LogWarning(
                    "Failed to resolve player ID for player {PlayerId} while processing scores for game {GameId}! This score will be missing!",
                    score.UserId,
                    osuApiGame.GameId
                );

                continue;
            }

            if (!await ScoreExistsInDatabaseAsync(osuApiGame.GameId, playerId.Value))
            {
                var dbMatchScore = new MatchScore
                {
                    PlayerId = playerId.Value,
                    GameId = dbGame.Id,
                    Team = (int)score.Team,
                    Score = score.PlayerScore,
                    MaxCombo = score.MaxCombo,
                    Count50 = score.Count50,
                    Count100 = score.Count100,
                    Count300 = score.Count300,
                    CountMiss = score.CountMiss,
                    CountKatu = score.CountKatu,
                    CountGeki = score.CountGeki,
                    Perfect = score.Perfect == 1,
                    Pass = score.Pass == 1,
                    EnabledMods = (int?)score.EnabledMods,
                    IsValid = true // We know this score is valid because we checked it above
                };

                await matchScoresRepository.CreateAsync(dbMatchScore);

                countSaved++;
            }
        }

        if (countSaved > 0)
        {
            logger.LogDebug("Saved {Count} new scores for game {GameId}", countSaved, osuApiGame.GameId);
        }
    }

    private async Task<bool> ScoreExistsInDatabaseAsync(long gameId, int playerId)
    {
        return await context.MatchScores.AnyAsync(x => x.Game.GameId == gameId && x.PlayerId == playerId);
    }
}
