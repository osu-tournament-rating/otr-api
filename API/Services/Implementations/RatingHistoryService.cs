using API.Configurations;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class RatingHistoryService : ServiceBase<RatingHistory>, IRatingHistoryService
{
	private readonly ILogger<RatingHistoryService> _logger;
	private readonly IPlayerService _playerService;

	public RatingHistoryService(ICredentials credentials, ILogger<RatingHistoryService> logger, IPlayerService playerService) :
		base(credentials, logger)
	{
		_logger = logger;
		_playerService = playerService;
	}

	public async Task<IEnumerable<RatingHistory>> GetForPlayerAsync(int playerId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<RatingHistory>("SELECT * FROM ratinghistories WHERE player_id = @PlayerId", new { PlayerId = playerId });
		}
	}

	public async Task ReplaceBatchAsync(IEnumerable<RatingHistory> histories)
	{
		histories = histories.OrderBy(x => x.Created);
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			await connection.ExecuteAsync("INSERT INTO ratinghistories (player_id, mu, sigma, created, mode, game_id) VALUES " +
			                              "(@PlayerId, @Mu, @Sigma, @Created, @Mode, @GameId) ON CONFLICT (player_id, game_id) " +
			                              "DO UPDATE SET player_id = @PlayerId, mu = @Mu, sigma = @Sigma, mode = @Mode, game_id = @GameId, " +
			                              "created = @Created, updated = current_timestamp",
				histories);
			_logger.LogInformation("Batch inserted {Count} rating histories", histories.Count());
		}
	}

	public async Task TruncateAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			await connection.ExecuteAsync("TRUNCATE TABLE ratinghistories");
		}
	}

	public async Task InsertAsync(IEnumerable<RatingHistory> histories)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			await connection.ExecuteAsync("INSERT INTO ratinghistories (player_id, mu, sigma, created, mode, game_id) VALUES (@PlayerId, @Mu, @Sigma, @Created, @Mode, @GameId)",
				histories.ToList());
		}
	}
}