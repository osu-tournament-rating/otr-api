using API.Entities;
using API.Enums;
using API.Osu;
using API.Osu.Multiplayer;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class ApiMatchService : IApiMatchService
{
	private readonly IBeatmapService _beatmapService;
	private readonly IOsuApiService _osuApiService;
	private readonly IMatchesService _matchesService;
	private readonly IGamesService _gamesService;
	private readonly IMatchScoresService _matchScoresService;
	private readonly OtrContext _context;
	private readonly ILogger<ApiMatchService> _logger;
	private readonly IPlayerService _playerService;

	/// <summary>
	/// Strictly responsible for processing matches from the osu! API and adding them to the database. This includes:
	/// * Player data
	/// * Beatmap data
	/// * Match data, game data, and score data
	/// </summary>
	public ApiMatchService(ILogger<ApiMatchService> logger, OtrContext context, IPlayerService playerService, 
		IBeatmapService beatmapService, IOsuApiService osuApiService, IMatchesService matchesService, IGamesService gamesService, IMatchScoresService matchScoresService)
	{
		_logger = logger;
		_context = context;
		_playerService = playerService;
		_beatmapService = beatmapService;
		_osuApiService = osuApiService;
		_matchesService = matchesService;
		_gamesService = gamesService;
		_matchScoresService = matchScoresService;
	}

	public async Task<Entities.Match?> CreateFromApiMatchAsync(OsuApiMatchData apiMatch)
	{
		_logger.LogInformation("Processing match {MatchId}", apiMatch.Match.MatchId);

		await CreatePlayersAsync(apiMatch);
		await CreateBeatmapsAsync(apiMatch);
		return await UpdateMatchDataAsync(apiMatch);
	}

	private async Task CreatePlayersAsync(OsuApiMatchData apiMatch)
	{
		var existingPlayersMapping = await ExistingPlayersMapping(apiMatch);

		if (existingPlayersMapping == null)
		{
			return;
		}

		foreach (long osuId in GetUserIdsFromMatch(apiMatch)!)
		{
			if (existingPlayersMapping.ContainsKey(osuId))
			{
				// No need to create the player, they already exist
				continue;
			}
			
			var newPlayer = new Player { OsuId = osuId };
			var player = await _playerService.CreateAsync(newPlayer);

			_logger.LogInformation("Saved new player: {PlayerId} (osuId: {OsuId}) from match {MatchId}", player?.Id, osuId, apiMatch.Match.MatchId);
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
		var osuPlayerIds = GetUserIdsFromMatch(apiMatch);

		if (!osuPlayerIds?.Any() ?? true)
		{
			_logger.LogError("No players found in match {MatchId}", apiMatch.Match.MatchId);
			return null;
		}

		// Select all players from the database that are in the match
		var existingPlayers = await _context.Players.Where(p => osuPlayerIds.Contains(p.OsuId)).ToListAsync();

		_logger.LogTrace("Identified {Count} existing players to add for match {MatchId}", existingPlayers.Count, apiMatch.Match.MatchId);

		// Map these osu! ids to their database ids
		return existingPlayers.ToDictionary(player => player.OsuId, player => player.Id);
	}

	private List<long>? GetUserIdsFromMatch(OsuApiMatchData apiMatch) => apiMatch.Games.SelectMany(x => x.Scores).Select(x => x.UserId).Distinct().ToList();

	// Beatmaps

	/// <summary>
	///  Saves the beatmaps identified in the match to the database. Only save complete beatmaps. This does call the osu! API.
	/// </summary>
	private async Task CreateBeatmapsAsync(OsuApiMatchData apiMatch)
	{
		var beatmapIds = GetBeatmapIds(apiMatch).Distinct().ToList();

		if (beatmapIds.Count == 0)
		{
			return;
		}
		
		var beatmapsToSave = new List<Beatmap>();
		int countSaved = 0;
		
		foreach(long beatmapId in beatmapIds)
		{
			var existingBeatmap = await _beatmapService.GetBeatmapByBeatmapIdAsync(beatmapId);
			if (existingBeatmap == null)
			{
				var beatmap = await _osuApiService.GetBeatmapAsync(beatmapId, $"Beatmap {beatmapId} from match {apiMatch.Match.MatchId} does not exist in database");

				if (beatmap != null)
				{
					beatmapsToSave.Add(beatmap);
					countSaved++;
				}
			}
		}

		await _beatmapService.BulkInsertAsync(beatmapsToSave);

		if (countSaved > 0)
		{
			_logger.LogInformation("Saved {Count} beatmaps from match {MatchId}", countSaved, apiMatch.Match.MatchId);
		}
	}

	private IEnumerable<long> GetBeatmapIds(OsuApiMatchData apiMatch) => apiMatch.Games.Select(x => x.BeatmapId);

	// Match
	/// <summary>
	/// Updates an existing database match with data from the osu! API.
	/// </summary>
	/// <param name="apiMatch"></param>
	/// <returns></returns>
	/// <exception cref="NullReferenceException">If the existing match is null</exception>
	/// <exception cref="InvalidOperationException">If the Match.IsApiProcessed flag is null or if it is true</exception>
	private async Task<Entities.Match?> UpdateMatchDataAsync(OsuApiMatchData apiMatch)
	{
		var existingMatch = await ExistingMatch(apiMatch);

		if (existingMatch == null)
		{
			throw new NullReferenceException($"Match {apiMatch.Match.MatchId} does not exist in database. This should not be possible as it should have been created by POST /api/matches/batch");
		}

		if (existingMatch.IsApiProcessed == null)
		{
			throw new InvalidOperationException($"Match {apiMatch.Match.MatchId} has no IsApiProcessed value! This should not be possible as it should have been set by POST /api/matches/batch");
		}

		if (existingMatch.IsApiProcessed.Value)
		{
			// The match has already been marked as api processed. This shouldn't be possible.
			throw new InvalidOperationException($"Match {apiMatch.Match.MatchId} has already been marked as api processed! This should not be possible as it should have been set by POST /api/matches/batch");
		}

		existingMatch = await UpdateMatchAsync(apiMatch, existingMatch);

		var persistedGames = await CreateGamesAsync(apiMatch.Games, existingMatch);
		foreach (var game in apiMatch.Games)
		{
			await CreateScoresAsync(game);
		}

		if (persistedGames.Count > 0)
		{
			_logger.LogInformation("Saved scores for {Count} games from match {MatchId}", persistedGames.Count, apiMatch.Match.MatchId);
		}
		
		// Fetch the full entity from the database once again to ensure we have the latest populated data
		return await _matchesService.GetAsync(existingMatch.Id);
	}

	private async Task<Entities.Match?> ExistingMatch(OsuApiMatchData apiMatch) => await _context.Matches.FirstOrDefaultAsync(x => x.MatchId == apiMatch.Match.MatchId);

	private async Task<Entities.Match> UpdateMatchAsync(OsuApiMatchData apiMatch, Entities.Match existingMatch)
	{
		existingMatch.Name = apiMatch.Match.Name;
		existingMatch.StartTime = apiMatch.Match.StartTime;
		existingMatch.EndTime = apiMatch.Match.EndTime;
		existingMatch.IsApiProcessed = true;

		await _matchesService.UpdateAsync(existingMatch);

		_logger.LogInformation("Updated match: {MatchId} (name: {MatchName})", existingMatch.MatchId, existingMatch.Name);
		return existingMatch;
	}

	// Games
	/// <summary>
	/// Persists all games from the osu! API to the database.
	/// </summary>
	/// <param name="osuMatchGames"></param>
	/// <param name="existingMatch"></param>
	/// <returns>List of persisted entities</returns>
	public async Task<IList<Entities.Game>> CreateGamesAsync(IEnumerable<Osu.Multiplayer.Game> osuMatchGames, Entities.Match existingMatch)
	{
		var persisted = new List<Entities.Game>();
		foreach (var game in osuMatchGames)
		{
			int? beatmapIdResult = await _beatmapService.GetIdByBeatmapIdAsync(game.BeatmapId);

			var existingGame = await _context.Games.FirstOrDefaultAsync(g => g.GameId == game.GameId);

			if (existingGame == null)
			{
				var newGame = await AddNewGameAsync(game, existingMatch, beatmapIdResult);

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
	/// <param name="game"></param>
	/// <param name="existingMatch"></param>
	/// <param name="beatmapIdResult"></param>
	private async Task<Entities.Game?> AddNewGameAsync(Osu.Multiplayer.Game game, Entities.Match existingMatch, int? beatmapIdResult)
	{
		var dbGame = new Entities.Game
		{
			MatchId = existingMatch.Id,
			GameId = game.GameId,
			StartTime = game.StartTime,
			EndTime = game.EndTime,
			BeatmapId = beatmapIdResult,
			PlayMode = (int)game.PlayMode,
			ScoringType = (int)game.ScoringType,
			TeamType = (int)game.TeamType,
			Mods = (int)game.Mods
		};

		var persisted = await _gamesService.CreateAsync(dbGame);
		_logger.LogDebug("Saved game {GameId}", dbGame.GameId);

		if (persisted == null)
		{
			_logger.LogError("Failed to save game {GameId}!", dbGame.GameId);
		}
		
		return persisted;
	}
	
	private async Task<Entities.Game?> GetGameFromDatabase(long gameId)
	{
		return await _context.Games.FirstOrDefaultAsync(x => x.GameId == gameId);
	}
	
	// Scores
	private async Task CreateScoresAsync(Osu.Multiplayer.Game game)
	{
		var dbGame = await GetGameFromDatabase(game.GameId);
		if (dbGame == null)
		{
			_logger.LogError("Failed to fetch game {GameId} from database while processing scores! This means {Count} scores will be missing for this game!", game.GameId, game.Scores.Count);
			return;
		}

		int countSaved = 0;
		foreach (var score in game.Scores)
		{
			int playerId = await _playerService.GetIdByOsuIdAsync(score.UserId);
			if (playerId == default)
			{
				_logger.LogWarning("Failed to resolve player ID for player {PlayerId} while processing scores for game {GameId}! This score will be missing!", score.UserId, game.GameId);
				continue;
			}

			if (!await ScoreExistsInDatabaseAsync(game.GameId, playerId))
			{
				var dbMatchScore = new MatchScore
				{
					PlayerId = playerId,
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

				await _matchScoresService.CreateAsync(dbMatchScore);

				countSaved++;
			}
		}

		if (countSaved > 0)
		{
			_logger.LogDebug("Saved {Count} new scores for game {GameId}", countSaved, game.GameId);
		}
	}

	private async Task<bool> ScoreExistsInDatabaseAsync(long gameId, int playerId)
	{
		return await _context.MatchScores.AnyAsync(x => x.Game.GameId == gameId && x.PlayerId == playerId);
	}
}