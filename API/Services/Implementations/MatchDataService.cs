using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class MatchDataService : ServiceBase<MatchData>, IMatchDataService
{
	public MatchDataService(IDbCredentials dbCredentials, ILogger<MatchDataService> logger) : base(dbCredentials, logger) {}

	public async Task<IEnumerable<MatchData>> GetFilteredDataAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			const int SCORE_THRESHOLD = 10000;
			const double ACCURACY_THRESHOLD = 0.3;
			
			return await connection.QueryAsync<MatchData>(
				"SELECT * FROM matchdata WHERE " +
				$"score > {SCORE_THRESHOLD} AND " +
				$"accuracy > {ACCURACY_THRESHOLD}");
		}
	}
}