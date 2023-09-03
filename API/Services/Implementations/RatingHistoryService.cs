using API.Configurations;
using API.Entities;
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
		/**
		 * USE THIS WITH CAUTION !!!
		 */

		// histories = histories.ToList();
		// var playerIds = (await _playerService.GetAllAsync() ?? throw new InvalidOperationException("Invalid ratings")).ToDictionary(x => x.OsuId, x => x.Id);
		//
		// foreach (var r in histories)
		// {
		// 	r.PlayerId = playerIds[r.PlayerId];
		// 	r.MatchDataId = ids[(r.PlayerId, r.GameId)];
		// }
		//
		// histories = histories.OrderBy(x => x.Created);
		// using (var connection = new NpgsqlConnection(ConnectionString))
		// {
		// 	await connection.ExecuteAsync("TRUNCATE TABLE ratinghistories");
		// 	await connection.ExecuteAsync("INSERT INTO ratinghistories (player_id, mu, sigma, created, mode, match_data_id) VALUES (@PlayerId, @Mu, @Sigma, @Created, @Mode, @MatchDataId)",
		// 		histories);
		// }
		await Task.Delay(1);
		throw new NotImplementedException();
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
			await connection.ExecuteAsync("INSERT INTO ratinghistories (player_id, mu, sigma, created, mode, match_data_id) VALUES (@PlayerId, @Mu, @Sigma, @Created, @Mode, @MatchDataId)",
				histories.ToList());
		}
	}
}