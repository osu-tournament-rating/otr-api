using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class OsuMatchService : ServiceBase<Match>, IMultiplayerLinkService
{
	public OsuMatchService(ICredentials credentials, ILogger<OsuMatchService> logger) : base(credentials, logger) {}

	public async Task<Match?> GetByLobbyIdAsync(long matchId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<Match?>("SELECT * FROM matches WHERE match_id = @lobbyId", new { lobbyId = matchId });
		}
	}

	public async Task<IEnumerable<Match>?> GetAllPendingVerificationAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<Match>($"SELECT * FROM matches WHERE verification_status = {VerificationStatus.PendingVerification:D}");
		}
	}

	public async Task<Match?> GetFirstPendingOrDefaultAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryFirstOrDefaultAsync<Match?>($"SELECT * FROM osumatches WHERE verification_status = {VerificationStatus.PendingVerification:D}");
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