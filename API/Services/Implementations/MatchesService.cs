using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class MatchesService : ServiceBase<Match>, IMatchesService
{
	public MatchesService(ICredentials credentials, ILogger<MatchesService> logger) : base(credentials, logger) {}

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
			return await connection.QueryFirstOrDefaultAsync<Match?>($"SELECT * FROM matches WHERE verification_status = {VerificationStatus.PendingVerification:D}");
		}
	}

	public async Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> matchIds)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<long>("SELECT match_id FROM matches WHERE match_id = ANY(@lobbyIds)", new { lobbyIds = matchIds });
		}
	}

	/// <summary>
	/// Used to queue up matches for verification.
	/// </summary>
	/// <returns>Number of rows inserted</returns>
	public async Task<int> InsertFromIdBatchAsync(IEnumerable<Match> matches)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.ExecuteAsync("INSERT INTO matches (match_id, verification_status) VALUES (@MatchId, @VerificationStatus)", matches);
		}
	}
}