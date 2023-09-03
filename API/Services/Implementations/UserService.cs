using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class UserService : ServiceBase<User>, IUserService
{
	public UserService(ICredentials credentials, ILogger<UserService> logger) : base(credentials, logger) {}

	public async Task<User> GetForPlayerAsync(int playerId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM users WHERE player_id = @PlayerId", new { PlayerId = playerId });
		}
	}
}