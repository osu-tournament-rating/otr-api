using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class OsuMatchService : ServiceBase<OsuMatch>, IMultiplayerLinkService
{
	public OsuMatchService(ICredentials credentials, ILogger<OsuMatchService> logger) : base(credentials, logger) {}

	public async Task<OsuMatch?> GetByLobbyIdAsync(long matchId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<OsuMatch?>("SELECT * FROM osumatches WHERE match_id = @lobbyId", new { lobbyId = matchId });
		}
	}

	public async Task<IEnumerable<OsuMatch>?> GetAllPendingVerificationAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<OsuMatch>($"SELECT * FROM osumatches WHERE verification_status = {VerificationStatus.PendingVerification:D}");
		}
	}

	public async Task<OsuMatch?> GetFirstPendingOrDefaultAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryFirstOrDefaultAsync<OsuMatch?>($"SELECT * FROM osumatches WHERE verification_status = {VerificationStatus.PendingVerification:D}");
		}
	}

	public async Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> matchIds)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<long>("SELECT match_id FROM osumatches WHERE match_id = ANY(@lobbyIds)", new { lobbyIds = matchIds });
		}
	}
}