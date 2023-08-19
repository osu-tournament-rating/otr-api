using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class MultiplayerLinkService : ServiceBase<MultiplayerLink>, IMultiplayerLinkService
{
	public MultiplayerLinkService(ICredentials credentials, ILogger<MultiplayerLinkService> logger) : base(credentials, logger) {}

	public async Task<MultiplayerLink?> GetByLobbyIdAsync(long lobbyId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<MultiplayerLink?>("SELECT * FROM multiplayerlinks WHERE mp_link_id = @lobbyId", new { lobbyId });
		}
	}

	public async Task<IEnumerable<MultiplayerLink>?> GetAllPendingAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<MultiplayerLink>("SELECT * FROM multiplayerlinks WHERE status = 'PENDING'");
		}
	}

	public async Task<MultiplayerLink?> GetFirstPendingOrDefaultAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryFirstOrDefaultAsync<MultiplayerLink?>("SELECT * FROM multiplayerlinks WHERE status = 'PENDING'");
		}
	}

	public async Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> lobbyIds)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<long>("SELECT mp_link_id FROM multiplayerlinks WHERE mp_link_id = ANY(@lobbyIds)", new { lobbyIds });
		}
	}
}