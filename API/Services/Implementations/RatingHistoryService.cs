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
}