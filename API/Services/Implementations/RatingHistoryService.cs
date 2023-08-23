using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class RatingHistoryService : ServiceBase<RatingHistory>, IRatingHistoryService
{
	private readonly ILogger<RatingHistoryService> _logger;
	private readonly IMatchDataService _matchDataService;
	private readonly IPlayerService _playerService;

	public RatingHistoryService(ICredentials credentials, ILogger<RatingHistoryService> logger, IMatchDataService matchDataService, IPlayerService playerService) :
		base(credentials, logger)
	{
		_logger = logger;
		_matchDataService = matchDataService;
		_playerService = playerService;
	}

	public async Task<IEnumerable<RatingHistory>> GetAllForPlayerAsync(int playerId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<RatingHistory>("SELECT * FROM ratinghistories WHERE player_id = @PlayerId", new { PlayerId = playerId });
		}
	}

	public async Task ReplaceBatchAsync(IEnumerable<RatingHistory> ratings)
	{
		/**
		 * USE THIS WITH CAUTION !!!
		 */

		ratings = ratings.ToList();
		var playerIds = (await _playerService.GetAllAsync() ?? throw new InvalidOperationException("Invalid ratings")).ToDictionary(x => x.OsuId, x => x.Id);
		var ids = (await _matchDataService.GetIdsPlayerIdsGameIdsAsync()).ToDictionary(x => (x.playerId, x.gameId), x => x.id);

		foreach (var r in ratings)
		{
			r.PlayerId = playerIds[r.PlayerId];
			r.MatchDataId = ids[(r.PlayerId, r.GameId)];
		}

		ratings = ratings.OrderBy(x => x.Created);
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			await connection.ExecuteAsync("TRUNCATE TABLE ratinghistories");
			await connection.ExecuteAsync("INSERT INTO ratinghistories (player_id, mu, sigma, created, mode, match_data_id) VALUES (@PlayerId, @Mu, @Sigma, @Created, @Mode, @MatchDataId)",
				ratings);
		}
	}

	public async Task TruncateAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			await connection.ExecuteAsync("TRUNCATE TABLE ratinghistories");
		}
	}
}