using API.Entities;
using API.Osu.Multiplayer;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Strictly responsible for processing matches from the osu! API and adding them to the database. This includes:
/// * Player data
/// * Beatmap data
/// * Match data, game data, and score data
/// </summary>
public class ApiMatchRepository(
    ILogger<ApiMatchRepository> logger,
    OtrContext context,
    IPlayerRepository playerRepository,
    IBeatmapRepository beatmapRepository,
    IOsuApiService osuApiService,
    IMatchesRepository matchesRepository,
    IGamesRepository gamesRepository,
    IMatchScoresRepository matchScoresRepository
    ) : IApiMatchRepository
{
    private readonly IBeatmapRepository _beatmapRepository = beatmapRepository;
    private readonly IOsuApiService _osuApiService = osuApiService;
    private readonly IMatchesRepository _matchesRepository = matchesRepository;
    private readonly IGamesRepository _gamesRepository = gamesRepository;
    private readonly IMatchScoresRepository _matchScoresRepository = matchScoresRepository;
    private readonly OtrContext _context = context;
    private readonly ILogger<ApiMatchRepository> _logger = logger;
    private readonly IPlayerRepository _playerRepository = playerRepository;

    public async Task<Match?> CreateFromApiMatchAsync(OsuApiMatchData apiMatch)
    {
        _logger.LogInformation("Processing match {MatchId}", apiMatch.OsuApiMatch.MatchId);

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

        foreach (long osuId in GetUserIdsFromMatch(apiMatch))
        {
            if (existingPlayersMapping.ContainsKey(osuId))
            {
                // No need to create the player, they already exist
                continue;
            }

            var newPlayer = new Player { OsuId = osuId };
            Player player = await _playerRepository.CreateAsync(newPlayer);

            _logger.LogInformation(
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
            _logger.LogError("No players found in match {MatchId}", apiMatch.OsuApiMatch.MatchId);
            return null;
        }

        // Select all players from the database that are in the match
        List<Player> existingPlayers = await _context.Players.Where(p => osuPlayerIds.Contains(p.OsuId)).ToListAsync();

        _logger.LogTrace(
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
            _logger.LogError("No beatmap IDs found in match {MatchId}", apiMatch.OsuApiMatch.MatchId);
            return;
        }

        var beatmapsToSave = new List<Beatmap>();
        int countSaved = 0;

        foreach (long beatmapId in beatmapIds)
        {
            Beatmap? existingBeatmap = await _beatmapRepository.GetByOsuIdAsync(beatmapId);
            if (existingBeatmap == null)
            {
                Beatmap? beatmap = await _osuApiService.GetBeatmapAsync(
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

        await _beatmapRepository.BulkInsertAsync(beatmapsToSave);

        if (countSaved > 0)
        {
            _logger.LogInformation(
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
            _logger.LogInformation(
                "Saved scores for {Count} games from match {MatchId}",
                persistedGames.Count,
                apiMatch.OsuApiMatch.MatchId
            );
        }

        // Fetch the full entity from the database once again to ensure we have the latest populated data
        return await _matchesRepository.GetAsync(existingMatch.Id);
    }

    private async Task<Match?> ExistingMatch(OsuApiMatchData apiMatch) =>
        await _context.Matches.FirstOrDefaultAsync(x => x.MatchId == apiMatch.OsuApiMatch.MatchId);

    private async Task<Match> UpdateMatchAsync(OsuApiMatchData apiMatch, Match existingMatch)
    {
        existingMatch.Name = apiMatch.OsuApiMatch.Name;
        existingMatch.StartTime = apiMatch.OsuApiMatch.StartTime;
        existingMatch.EndTime = apiMatch.OsuApiMatch.EndTime;
        existingMatch.IsApiProcessed = true;
        existingMatch.VerificationInfo = null;

        await _matchesRepository.UpdateAsync(existingMatch);

        _logger.LogInformation(
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
            int? beatmapIdResult = await _beatmapRepository.GetIdByBeatmapIdAsync(game.BeatmapId);

            Game? existingGame = await _context.Games.FirstOrDefaultAsync(g => g.GameId == game.GameId);

            if (existingGame == null)
            {
                Game? newGame = await AddNewGameAsync(game, existingMatch, beatmapIdResult);

                if (newGame == null)
                {
                    // Something seriously went wrong.
                    _logger.LogError("Failed to save new game {GameId}!", game.GameId);
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
            EndTime = osuApiGame.EndTime,
            BeatmapId = beatmapIdResult,
            PlayMode = (int)osuApiGame.PlayMode,
            ScoringType = (int)osuApiGame.ScoringType,
            TeamType = (int)osuApiGame.TeamType,
            Mods = (int)osuApiGame.Mods
        };

        Game persisted = await _gamesRepository.CreateAsync(dbGame);
        _logger.LogDebug("Saved game {GameId}", dbGame.GameId);

        return persisted;
    }

    private async Task<Game?> GetGameFromDatabase(long gameId)
    {
        return await _context.Games.FirstOrDefaultAsync(x => x.GameId == gameId);
    }

    // Scores
    private async Task CreateScoresAsync(OsuApiGame osuApiGame)
    {
        Game? dbGame = await GetGameFromDatabase(osuApiGame.GameId);
        if (dbGame == null)
        {
            _logger.LogError(
                "Failed to fetch game {GameId} from database while processing scores! This means {Count} scores will be missing for this game!",
                osuApiGame.GameId,
                osuApiGame.Scores.Count
            );

            return;
        }

        int countSaved = 0;
        foreach (OsuApiScore score in osuApiGame.Scores)
        {
            int? playerId = await _playerRepository.GetIdAsync(score.UserId);
            if (!playerId.HasValue)
            {
                _logger.LogWarning(
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

                await _matchScoresRepository.CreateAsync(dbMatchScore);

                countSaved++;
            }
        }

        if (countSaved > 0)
        {
            _logger.LogDebug("Saved {Count} new scores for game {GameId}", countSaved, osuApiGame.GameId);
        }
    }

    private async Task<bool> ScoreExistsInDatabaseAsync(long gameId, int playerId)
    {
        return await _context.MatchScores.AnyAsync(x => x.Game.GameId == gameId && x.PlayerId == playerId);
    }
}
