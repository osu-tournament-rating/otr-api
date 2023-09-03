using API.Configurations;
using API.Entities;
using API.Osu.Multiplayer;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class MatchesService : ServiceBase<Entities.Match>, IMatchesService
{
	private readonly IBeatmapService _beatmapService;
	private readonly IGamesService _gamesService;
	private readonly ILogger<MatchesService> _logger;
	private readonly IPlayerService _playerService;
	private readonly IMatchScoresService _scoresService;

	public MatchesService(ICredentials credentials, ILogger<MatchesService> logger, IGamesService gamesService, IMatchScoresService scoresService,
		IBeatmapService beatmapService, IPlayerService playerService) : base(credentials, logger)
	{
		_logger = logger;
		_gamesService = gamesService;
		_scoresService = scoresService;
		_beatmapService = beatmapService;
		_playerService = playerService;
	}

	public async Task<Entities.Match?> GetByLobbyIdAsync(long matchId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<Entities.Match?>("SELECT * FROM matches WHERE match_id = @lobbyId", new { lobbyId = matchId });
		}
	}

	public async Task<IEnumerable<Entities.Match>?> GetAllPendingVerificationAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<Entities.Match>($"SELECT * FROM matches WHERE verification_status = {VerificationStatus.PendingVerification:D}");
		}
	}

	public async Task<Entities.Match?> GetFirstPendingOrDefaultAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryFirstOrDefaultAsync<Entities.Match?>($"SELECT * FROM matches WHERE verification_status = {VerificationStatus.PendingVerification:D}");
		}
	}

	public async Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> matchIds)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<long>("SELECT match_id FROM matches WHERE match_id = ANY(@lobbyIds)", new { lobbyIds = matchIds });
		}
	}

	public async Task<int> InsertFromIdBatchAsync(IEnumerable<Entities.Match> matches)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.ExecuteAsync("INSERT INTO matches (match_id, verification_status) VALUES (@MatchId, @VerificationStatus)", matches);
		}
	}

	public async Task<int?> CreateIfNotExistsAsync(Entities.Match match)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<int?>(
				"INSERT INTO matches (match_id, verification_status) VALUES (@MatchId, @VerificationStatus) ON CONFLICT (match_id) DO NOTHING RETURNING match_id", match);
		}
	}

	public async Task<bool> CreateFromApiMatchAsync(OsuApiMatchData osuMatch)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			try
			{
				// Step 1: Fetch the player ids that played in the match and map them to the database ids.
				// Step 2: Check for the existing beatmap, if not insert it. Then map the beatmap ids to the database ids.
				// Step 3: Insert the match if it doesn't exist.
				// Step 4: Insert the games if they don't exist.
				// Step 5: Insert the scores if they don't exist.

				// Step 1.
				var osuPlayerIds = osuMatch.Games.SelectMany(x => x.Scores).Select(x => x.UserId).Distinct().ToList();
				var existingPlayers = (await _playerService.GetByOsuIdsAsync(osuPlayerIds)).ToList();
				
				var playerIdMapping = new Dictionary<long, int>();
				
				// Create the players that don't exist
				var playersToCreate = osuPlayerIds.Except(existingPlayers.Select(x => x.OsuId)).ToList();
				foreach (long playerId in playersToCreate)
				{
					var player = new Player { OsuId = playerId };
					int? playerIdDb = await _playerService.CreateAsync(player);
					if (playerIdDb == null)
					{
						_logger.LogError("Failed to insert player {PlayerId}", playerId);
						continue;
					}
					
					playerIdMapping.Add(player.OsuId, playerIdDb.Value);
				}
				
				foreach (var player in existingPlayers)
				{
					playerIdMapping.Add(player.OsuId, player.Id);
				}

				// Step 2.
				var osuBeatmapIds = osuMatch.Games.Select(x => x.BeatmapId).Distinct().ToList();
				var existingBeatmaps = (await _beatmapService.GetByBeatmapIdsAsync(osuBeatmapIds)).ToList();
				
				var beatmapIdMapping = new Dictionary<long, int>();
				foreach (var beatmap in existingBeatmaps)
				{
					beatmapIdMapping.Add(beatmap.BeatmapId, beatmap.Id);
				}

				// Step 3.
				var dbMatch = new Entities.Match
				{
					MatchId = osuMatch.Match.MatchId,
					Name = osuMatch.Match.Name,
					StartTime = osuMatch.Match.StartTime,
					EndTime = osuMatch.Match.EndTime,
					VerificationStatus = VerificationStatus.PendingVerification
				};

				int? matchId = await connection.QuerySingleOrDefaultAsync<int?>(
					"INSERT INTO matches (match_id, name, start_time, end_time, verification_info, verification_source, verification_status) VALUES " +
					"(@MatchId, @Name, @StartTime, @EndTime, @VerificationInfo, @VerificationSource, @VerificationStatus) " +
					"ON CONFLICT (match_id) DO UPDATE SET name = @Name, start_time = @StartTime, end_time = @EndTime, verification_info = @VerificationInfo, " +
					"verification_source = @VerificationSource, verification_status = @VerificationStatus " +
					"RETURNING id", dbMatch);

				if (matchId == null)
				{
					_logger.LogError("Failed to insert match {MatchId} (issue with insertion of {@DbMatch})", osuMatch.Match.MatchId, dbMatch);
					return false;
				}

				// Step 4.
				foreach (var game in osuMatch.Games)
				{
					if (!beatmapIdMapping.TryGetValue(game.BeatmapId, out int _))
					{
						_logger.LogWarning("Failed to map beatmap id {BeatmapId} to database id", game.BeatmapId);
					}

					var dbGame = new Entities.Game
					{
						GameId = game.GameId,
						StartTime = game.StartTime,
						EndTime = game.EndTime,
						BeatmapId = 0,
						PlayMode = game.PlayMode,
						MatchId = matchId.Value
					};

					// Insert game into database and store the ids
					int rowsAffected = await _gamesService.CreateIfNotExistsAsync(dbGame);
					if (rowsAffected == 0)
					{
						_logger.LogError("Failed to insert game {GameId}", game.GameId);
					}
				}

				// Step 5.
				foreach (var game in osuMatch.Games)
				{
					var dbGame = await _gamesService.GetByGameIdAsync(game.GameId);
					if (dbGame == null)
					{
						_logger.LogError("Failed to fetch game {GameId}", game.GameId);
						continue;
					}

					foreach (var score in game.Scores)
					{
						var dbScore = new MatchScore
						{
							PlayerId = playerIdMapping[score.UserId],
							GameId = dbGame.Id,
							Team = score.Team,
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
							EnabledMods = score.EnabledMods
						};

						int? scoreId = await _scoresService.CreateIfNotExistsAsync(dbScore);
						if (scoreId == null)
						{
							_logger.LogError("Failed to insert score for player {PlayerId} in game {GameId} on map {Map} with score {Score}",
								dbScore.PlayerId, dbScore.GameId, dbGame.BeatmapId, dbScore.Score);
						}
					}
				}

				return true;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "An unhandled exception occurred while processing match {MatchId}", osuMatch.Match.MatchId);
				return false;
			}
		}
	}

	public async Task<int> UpdateVerificationStatusAsync(long matchId, VerificationStatus status, MatchVerificationSource source, string? info = null)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.ExecuteAsync("UPDATE matches SET verification_status = @Status, verification_source = @Source, verification_info = @Info WHERE match_id = @MatchId",
				new { MatchId = matchId, Status = status, Source = source, Info = info });
		}
	}

	public async Task<IEnumerable<Entities.Match>> GetForPlayerAsync(int playerId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			// 1. Retrieve the matches for the player
			const string matchSql = @"SELECT m.* FROM matches m
                                  INNER JOIN games g ON m.id = g.match_id
                                  INNER JOIN match_scores ms ON g.id = ms.game_id
                                  WHERE ms.player_id = @PlayerId";
			var matches = (await connection.QueryAsync<Entities.Match>(matchSql, new { PlayerId = playerId })).DistinctBy(x => x.MatchId).ToList();

			// 2. For each match, retrieve the associated games
			const string gameSql = @"SELECT g.* FROM games g
                                 WHERE g.match_id = @MatchId";
			
			foreach (var match in matches)
			{
				match.Games = (await connection.QueryAsync<Entities.Game>(gameSql, new { MatchId = match.Id })).ToList();

				// 3. For each game, retrieve the associated scores for the player
				const string scoreSql = @"SELECT ms.* FROM match_scores ms
                                      WHERE ms.game_id = @GameId AND ms.player_id = @PlayerId";
				foreach (var game in match.Games)
				{
					game.Scores = (await connection.QueryAsync<MatchScore>(scoreSql, new { GameId = game.Id, PlayerId = playerId })).ToList();
				}
			}

			return matches;
		}
	}

}