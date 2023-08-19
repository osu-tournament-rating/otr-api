using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class PlayerService : ServiceBase<Player>, IPlayerService
{
	public PlayerService(ICredentials credentials, ILogger<PlayerService> logger) : base(credentials, logger) {}

	public async Task<Player?> GetByOsuIdAsync(int osuId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<Player>("SELECT * FROM players WHERE osu_id = @OsuId", new { OsuId = osuId });
		}
	}

	public async Task<int> GetIdByOsuIdAsync(int osuId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<int>("SELECT id FROM players WHERE osu_id = @OsuId", new { OsuId = osuId });
		}
	}
}