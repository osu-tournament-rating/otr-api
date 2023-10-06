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
	private readonly OtrContext _context;
	private readonly ILogger<ApiMatchService> _logger;
	private readonly IPlayerService _playerService;
	
	private readonly OsuEnums.Mods[] _unallowedMods =
	{
		OsuEnums.Mods.TouchDevice,
		OsuEnums.Mods.SuddenDeath,
		OsuEnums.Mods.Relax,
		OsuEnums.Mods.Autoplay,
		OsuEnums.Mods.SpunOut,
		OsuEnums.Mods.Relax2, // Autopilot
		OsuEnums.Mods.Perfect,
		OsuEnums.Mods.Random,
		OsuEnums.Mods.Cinema,
		OsuEnums.Mods.Target
	};

	public ApiMatchService(ILogger<ApiMatchService> logger, OtrContext context, IPlayerService playerService, IBeatmapService beatmapService, IOsuApiService osuApiService)
	{
		_logger = logger;
		_context = context;
		_playerService = playerService;
		_beatmapService = beatmapService;
		_osuApiService = osuApiService;
	}

	public async Task CreateFromApiMatchAsync(OsuApiMatchData apiMatch)
	{
		_logger.LogInformation("Processing match {MatchId}", apiMatch.Match.MatchId);

		await CreatePlayersAsync(apiMatch);
		await CreateBeatmapsAsync(apiMatch);
		await ProcessMatchAsync(apiMatch);
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

			_logger.LogInformation("Saved new player: {PlayerId} (osuId: {OsuId}) from match {MatchId}", player.Id, osuId, apiMatch.Match.MatchId);
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
		var beatmapIds = GetBeatmapIds(apiMatch).ToList();
		
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
		_logger.LogInformation("Saved {Count} beatmaps from match {MatchId}", countSaved, apiMatch.Match.MatchId);
	}

	private IEnumerable<long> GetBeatmapIds(OsuApiMatchData apiMatch) => apiMatch.Games.Select(x => x.BeatmapId);

	// Match
	private async Task ProcessMatchAsync(OsuApiMatchData apiMatch)
	{
		var existingMatch = await ExistingMatch(apiMatch);

		if (existingMatch == null)
		{
			await CreatePendingMatchAsync(apiMatch);
		}
		else if (MatchNeedsPopulation(apiMatch, existingMatch))
		{
			await UpdateMatchAsync(apiMatch, existingMatch);
		}

		existingMatch = await ExistingMatch(apiMatch);
		// If the match is still null, something is seriously wrong.

		if (existingMatch == null)
		{
			_logger.LogError("Match {MatchId} is null after processing! This should never happen! Aborting adding of games & associated scores!", apiMatch.Match.MatchId);
			return;
		}

		await ProcessGamesAsync(apiMatch.Games, existingMatch);
		foreach(var game in apiMatch.Games)
		{
			await ProcessScoresAsync(game);
		}
	}

	private async Task<Entities.Match?> ExistingMatch(OsuApiMatchData apiMatch) => await _context.Matches.FirstOrDefaultAsync(x => x.MatchId == apiMatch.Match.MatchId);

	/// <summary>
	///  If the existing match is missing substantial data, it needs to be populated.
	/// </summary>
	/// <param name="apiMatch"></param>
	/// <param name="existingMatch"></param>
	/// <returns></returns>
	private bool MatchNeedsPopulation(OsuApiMatchData apiMatch, Entities.Match existingMatch)
	{
		if (apiMatch.Match.MatchId != existingMatch.MatchId)
		{
			_logger.LogError("Match ID mismatch during processing: {ApiMatchId} vs {ExistingMatchId}", apiMatch.Match.MatchId, existingMatch.MatchId);
			return false;
		}

		return existingMatch.Name == null || existingMatch.Name != apiMatch.Match.Name;
	}

	private async Task CreatePendingMatchAsync(OsuApiMatchData apiMatch)
	{
		var match = new Entities.Match
		{
			MatchId = apiMatch.Match.MatchId,
			Name = apiMatch.Match.Name,
			StartTime = apiMatch.Match.StartTime,
			EndTime = apiMatch.Match.EndTime,
			VerificationStatus = DetermineVerificationStatus(apiMatch)
		};

		_context.Matches.Add(match);
		await _context.SaveChangesAsync();

		_logger.LogInformation("Saved new match: {MatchId} (name: {MatchName})", match.MatchId, match.Name);
	}

	private int DetermineVerificationStatus(OsuApiMatchData apiMatch)
	{
		var existingMatch = ExistingMatch(apiMatch).Result;
		if (existingMatch?.VerificationStatusEnum == MatchVerificationStatus.Verified)
		{
			return (int)MatchVerificationStatus.Verified;
		}

		return (int)MatchVerificationStatus.PendingVerification;
	}

	private async Task UpdateMatchAsync(OsuApiMatchData apiMatch, Entities.Match existingMatch)
	{
		existingMatch.Name = apiMatch.Match.Name;
		existingMatch.StartTime = apiMatch.Match.StartTime;
		existingMatch.EndTime = apiMatch.Match.EndTime;
		existingMatch.VerificationStatus = DetermineVerificationStatus(apiMatch);

		_context.Matches.Update(existingMatch);
		await _context.SaveChangesAsync();

		_logger.LogInformation("Updated match: {MatchId} (name: {MatchName})", existingMatch.MatchId, existingMatch.Name);
	}

	// Games
	public async Task ProcessGamesAsync(IEnumerable<Osu.Multiplayer.Game> osuMatchGames, Entities.Match existingMatch)
	{
		foreach (var game in osuMatchGames)
		{
			int? beatmapIdResult = await GetBeatmapDatabaseIdAsync(game);

			var existingGame = await _context.Games.FirstOrDefaultAsync(g => g.GameId == game.GameId);

			if (existingGame == null)
			{
				await AddNewGameAsync(game, existingMatch, beatmapIdResult);
			}
			else
			{
				if (existingGame.BeatmapId == null && beatmapIdResult != null)
				{
					existingGame.BeatmapId = beatmapIdResult;
					_context.Games.Update(existingGame);
					await _context.SaveChangesAsync();
					
					_logger.LogInformation("Game had missing beatmap ID, updated game {GameId} to include beatmap id {BeatmapId}", existingGame.GameId, beatmapIdResult);
				}
			}
		}
	}

	private async Task<int?> GetBeatmapDatabaseIdAsync(Osu.Multiplayer.Game game)
	{
		int? id = await _beatmapService.GetIdByBeatmapIdAsync(game.BeatmapId);
		return id;
	}

	private async Task AddNewGameAsync(Osu.Multiplayer.Game game, Entities.Match existingMatch, int? beatmapIdResult)
	{
		var gameRejectionReason = CheckForGameRejection(game, existingMatch);

		var gameVerificationStatus = existingMatch.VerificationStatusEnum == MatchVerificationStatus.Verified
			? GameVerificationStatus.Verified
			: GameVerificationStatus.PreVerified;

		if (gameRejectionReason.HasValue)
		{
			gameVerificationStatus = GameVerificationStatus.Rejected;
			_logger.LogInformation("Game {GameId} was rejected for reason {Reason}", game.GameId, gameRejectionReason.Value);
		}

		var dbGame = new Entities.Game
		{
			MatchId = existingMatch.Id,
			GameId = game.GameId,
			StartTime = game.StartTime,
			EndTime = game.EndTime,
			BeatmapId = beatmapIdResult,
			PlayMode = (int)game.PlayMode,
			MatchType = (int)game.MatchType,
			ScoringType = (int)game.ScoringType,
			TeamType = (int)game.TeamType,
			Mods = (int)game.Mods,
			VerificationStatus = (int)gameVerificationStatus,
			RejectionReason = gameRejectionReason.HasValue ? (int?)gameRejectionReason.Value : null
		};

		await _context.Games.AddAsync(dbGame);
		await _context.SaveChangesAsync();

		_logger.LogDebug("Saved game {GameId}", dbGame.GameId);
	}
	
	private GameRejectionReason? CheckForGameRejection(Osu.Multiplayer.Game game, Entities.Match existingMatch)
	{
		GameRejectionReason? result = null;

		if(!GameHasCorrectTeamSize(game, existingMatch))
		{
			result = GameRejectionReason.TeamSizeMismatch;
		}
		else if (GameHasBadMods(game) || GameScoresHaveBadMods(game))
		{
			result = GameRejectionReason.BadMods;
		}
		else if (!GameHasCorrectTeamType(game, existingMatch))
		{
			result = GameRejectionReason.TeamTypeMismatch;
		}
		else if (!GameHasCorrectMode(game, existingMatch))
		{
			result = GameRejectionReason.GameModeMismatch;
		}

		return result;
	}

	private bool GameHasCorrectTeamSize(Osu.Multiplayer.Game game, Entities.Match match)
	{
		int? teamSize = match.TeamSize;
		if (teamSize == null)
		{
			_logger.LogInformation("Match {MatchId} has no team size, can't verify game {GameId}", match.MatchId, game.GameId);
			return false; // We can't verify a game if we don't know the team size
		}

		return (game.Scores.Count / 2) == teamSize;
	}

	private bool GameHasBadMods(Osu.Multiplayer.Game game) => _unallowedMods.Any(unallowedMod => game.Mods.HasFlag(unallowedMod));
	private bool GameScoresHaveBadMods(Osu.Multiplayer.Game game) => game.Scores.Any(score => 
		score.EnabledMods.HasValue &&
		_unallowedMods.Any(unallowedMod => score.EnabledMods.Value.HasFlag(unallowedMod))
	);

	private bool GameHasCorrectTeamType(Osu.Multiplayer.Game game, Entities.Match match)
	{
		int? expectedTeamSize = match.TeamSize;
		if (expectedTeamSize == null)
		{
			_logger.LogWarning("Match {MatchId} has no team size, can't verify game {GameId}", match.MatchId, game.GameId);
			return false; // We can't verify a game if we don't know the team size
		}

		if (expectedTeamSize <= 0)
		{
			_logger.LogWarning("Encountered unexpected team size {TeamSize} for match {MatchId}", expectedTeamSize, match.MatchId);
			return false; // Invalid expected team size
		}

		if (expectedTeamSize == 1)
		{
			return game.TeamType == OsuEnums.TeamType.HeadToHead;
		}

		return game.TeamType == OsuEnums.TeamType.TeamVs; // We only consider TeamVs
	}

	private bool GameHasCorrectMode(Osu.Multiplayer.Game game, Entities.Match match)
	{
		int? expectedMode = match.Mode;
		if (expectedMode == null)
		{
			_logger.LogWarning("Match {MatchId} has no mode, can't verify game {GameId}", match.MatchId, game.GameId);
			return false; // We can't verify a game if we don't know the mode
		}

		if (expectedMode is < 0 or > 3)
		{
			_logger.LogWarning("Mode for match {MatchId} is invalid: {Mode}", match.MatchId, expectedMode);
			return false;
		}

		return (int) game.PlayMode == expectedMode;
	}
	
	private async Task<Entities.Game?> GetGameFromDatabase(long gameId)
	{
		return await _context.Games.FirstOrDefaultAsync(x => x.GameId == gameId);
	}
	
	// Scores
	private async Task ProcessScoresAsync(Osu.Multiplayer.Game game)
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
			if (!ScoreIsValid(score))
			{
				_logger.LogDebug("Skipping processing of score [Player: {PlayerId} // Score: {ScoreAmount} // Game: {GameId}] because it is not valid!", score.UserId, score.PlayerScore, game.GameId);
				continue;
			}
			
			int playerId = await _playerService.GetIdByOsuIdAsync(score.UserId);
			if(playerId == default)
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

				_context.MatchScores.Add(dbMatchScore);
				await _context.SaveChangesAsync();

				countSaved++;
			}
		}
		
		_logger.LogDebug("Saved {Count} scores for game {GameId}", countSaved, game.GameId);
	}

	private bool ScoreIsValid(Score score)
	{
		return score.PlayerScore > 0;
	}
	
	private async Task<bool> ScoreExistsInDatabaseAsync(long gameId, int playerId)
	{
		return await _context.MatchScores.AnyAsync(x => x.GameId == gameId && x.PlayerId == playerId);
	}
}