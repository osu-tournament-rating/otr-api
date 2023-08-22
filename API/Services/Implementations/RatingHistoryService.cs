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

	public async Task AddBatchAsync(IEnumerable<RatingHistory> histories)
	{
		histories = histories.ToList();
		var players = await _playerService.GetAllAsync();
		var player_ids = players!.ToDictionary(x => x.OsuId, x => x.Id);
		var ids_player_ids_game_ids = (await _matchDataService.GetIdsPlayerIdsGameIdsAsync()).ToDictionary(x => (x.playerId, x.gameId), x => x.id);

		foreach (var h in histories)
		{
			try
			{
				h.PlayerId = player_ids[h.PlayerId];
				h.MatchDataId = ids_player_ids_game_ids[(h.PlayerId, h.GameId)];
			}
			catch (Exception e)
			{
				_logger.LogWarning("Missing data for ({PlayerId}, {GameId}) [{Message}]", h.PlayerId, h.GameId, e.Message);
			}
		}
		
		using(var connection = new NpgsqlConnection(ConnectionString))
		{
			await connection.ExecuteAsync("INSERT INTO ratinghistories (player_id, mu, sigma, created, mode, match_data_id) VALUES (@PlayerId, @Mu, @Sigma, @Created, @Mode, @MatchDataId)", histories);
		}
	}
}