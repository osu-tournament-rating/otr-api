using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class RatingHistoryService : ServiceBase<RatingHistory>, IRatingHistoryService
{
	protected RatingHistoryService(IDbCredentials dbCredentials, ILogger<RatingHistoryService> logger) : base(dbCredentials, logger) {}

	public async Task<IEnumerable<RatingHistory>> GetAllForPlayerAsync(int playerId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<RatingHistory>("SELECT * FROM ratinghistories WHERE player_id = @PlayerId", new { PlayerId = playerId });
		}
	}
}