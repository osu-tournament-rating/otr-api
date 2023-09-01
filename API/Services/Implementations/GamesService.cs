using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;
using NpgsqlTypes;

namespace API.Services.Implementations;

public class GamesService : ServiceBase<Game>, IGamesService
{
	private readonly IMatchScoresService _matchScoresService;
	public GamesService(ICredentials credentials, ILogger<GamesService> logger, IMatchScoresService matchScoresService) : base(credentials, logger)
	{
		_matchScoresService = matchScoresService;
	}

	public async Task<IEnumerable<Game>> GetForPlayerAsync(int playerId)
	{
		var scores = await _matchScoresService.GetForPlayerAsync(playerId);
		return scores.Select(x => x.Game);
	}

	public async Task<ulong> BulkInsertAsync(IEnumerable<Game> games)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			await connection.OpenAsync();
			using(var writer = connection.BeginBinaryImport(
				"COPY games (match_id, beatmap_id, play_mode, match_type, scoring_type, team_type, mods, game_id, start_time, end_time) FROM STDIN (FORMAT BINARY)"))
			{
				await writer.StartRowAsync();

				foreach (var game in games)
				{
					await writer.WriteAsync(game.MatchId, NpgsqlDbType.Integer);
					await writer.WriteAsync(game.BeatmapId, NpgsqlDbType.Integer);
					await writer.WriteAsync((int)game.PlayMode, NpgsqlDbType.Integer);
					await writer.WriteAsync((int)game.MatchType, NpgsqlDbType.Integer);
					await writer.WriteAsync((int)game.ScoringType, NpgsqlDbType.Integer);
					await writer.WriteAsync((int)game.TeamType, NpgsqlDbType.Integer);
					await writer.WriteAsync((int)game.Mods, NpgsqlDbType.Integer);
					await writer.WriteAsync(game.GameId, NpgsqlDbType.Bigint);
					await writer.WriteAsync(game.StartTime, NpgsqlDbType.Timestamp);
					await writer.WriteAsync(game.EndTime, NpgsqlDbType.Timestamp);
				}

				return await writer.CompleteAsync();
			}
		}
	}

	public Task<IEnumerable<Game>> GetByGameIdsAsync(IEnumerable<int> gameIds)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return connection.QueryAsync<Game>("SELECT * FROM games WHERE id = ANY(@GameIds)", new { GameIds = gameIds });
		}
	}

	public Task<Dictionary<long, int>> GetGameIdMappingAsync(IEnumerable<long> beatmapIds)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return connection.QueryAsync<long, int, KeyValuePair<long, int>>("SELECT beatmap_id, id FROM games WHERE beatmap_id = ANY(@BeatmapIds)", (beatmapId, gameId) => new KeyValuePair<long, int>(beatmapId, gameId), new { BeatmapIds = beatmapIds })
				.ContinueWith(x => x.Result.ToDictionary(y => y.Key, y => y.Value));
		}
	}
}