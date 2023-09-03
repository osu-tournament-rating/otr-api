using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class MatchScoresService : ServiceBase<MatchScore>, IMatchScoresService
{
	public MatchScoresService(ICredentials credentials, ILogger<MatchScoresService> logger) : base(credentials, logger) {}

	public async Task<IEnumerable<MatchScore>> GetForGameAsync(long gameId)
	{
		using(var connection = new NpgsqlConnection(ConnectionString))
		{
			var scores = await connection.QueryAsync<MatchScore>("SELECT * FROM match_scores WHERE game_id = @GameId", new { GameId = gameId });
			return await Task.WhenAll(scores.Select(FetchRelationshipsAsync));
		}
	}

	public async Task<IEnumerable<MatchScore>> GetForPlayerAsync(long playerId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			var scores = await connection.QueryAsync<MatchScore>("SELECT * FROM match_scores WHERE player_id = @PlayerId", new { PlayerId = playerId });
			return await Task.WhenAll(scores.Select(FetchRelationshipsAsync));
		}
	}

	public async Task<int> BulkInsertAsync(IEnumerable<MatchScore> matchScores)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.ExecuteAsync(
				"INSERT INTO match_scores (game_id, team, score, max_combo, count_50, count_100, count_300, count_miss, perfect, pass, enabled_mods, count_katu, count_geki, player_id) " +
				"VALUES (@GameId, @Team, @Score, @MaxCombo, @Count50, @Count100, @Count300, @CountMiss, @Perfect, @Pass, @EnabledMods, @CountKatu, @CountGeki, @PlayerId)", matchScores);
		}
	}

	public async Task<int?> CreateIfNotExistsAsync(MatchScore dbScore)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			int? existing = await connection.QuerySingleOrDefaultAsync<int?>("SELECT id FROM match_scores WHERE game_id = @GameId AND player_id = @PlayerId", new { dbScore.GameId, dbScore.PlayerId });
			if (existing != null)
			{
				return existing;
			}

			return await connection.QuerySingleOrDefaultAsync<int?>(
				"INSERT INTO match_scores (game_id, team, score, max_combo, count_50, count_100, count_300, count_miss, perfect, pass, enabled_mods, count_katu, count_geki, player_id) " +
				"VALUES (@GameId, @Team, @Score, @MaxCombo, @Count50, @Count100, @Count300, @CountMiss, @Perfect, @Pass, @EnabledMods, @CountKatu, @CountGeki, @PlayerId) RETURNING id", dbScore);
		}
	}

	private async Task<MatchScore> FetchRelationshipsAsync(MatchScore matchScore)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			matchScore.Game = await connection.QuerySingleOrDefaultAsync<Game>("SELECT * FROM games WHERE id = @Id", new { Id = matchScore.GameId });
			return matchScore;
		}
	}
}