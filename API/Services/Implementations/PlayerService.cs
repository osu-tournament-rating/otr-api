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

	public async Task<int> GetIdByOsuIdAsync(long osuId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<int>("SELECT id FROM players WHERE osu_id = @OsuId", new { OsuId = osuId });
		}
	}

	public async Task<long> GetOsuIdByIdAsync(int id)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<long>("SELECT osu_id FROM players WHERE id = @Id", new { Id = id });
		}
	}

	public async Task<Dictionary<long, int>> GetIdsByOsuIdsAsync(IEnumerable<long> osuIds)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			var results = await connection.QueryAsync<(long osuId, int id)>("SELECT osu_id, id FROM players WHERE osu_id = ANY(@OsuIds)", new { OsuIds = osuIds.ToArray() });
			return results.ToDictionary(x => x.osuId, x => x.id);
		}
	}

}