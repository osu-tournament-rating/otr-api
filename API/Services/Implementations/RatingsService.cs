using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class RatingsService : ServiceBase<Rating>, IRatingsService
{
	public RatingsService(IDbCredentials dbCredentials, ILogger<RatingsService> logger) : base(dbCredentials, logger) {}

	public async Task<IEnumerable<Rating>> GetAllForPlayerAsync(int id)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<Rating>("SELECT * FROM ratings WHERE player_id = @PlayerId", new { PlayerId = id });
		}
	}
}