using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class MatchDataService : ServiceBase<MatchData>, IMatchDataService
{
	private const int SCORE_THRESHOLD = 10000;
	private const double ACCURACY_THRESHOLD = 0.3;
	private readonly string _filterQuery = $"SELECT * FROM matchdata WHERE score > {SCORE_THRESHOLD} AND accuracy > {ACCURACY_THRESHOLD}";
	
	public MatchDataService(IDbCredentials dbCredentials, ILogger<MatchDataService> logger) : base(dbCredentials, logger) {}

	public async Task<IEnumerable<MatchData>> GetFilteredDataAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<MatchData>(_filterQuery);
		}
	}

	public async Task<IEnumerable<MatchData>> GetAllForPlayerAsync(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<MatchData>(_filterQuery + " AND player_id = @userId", new { userId });
		}
	}
}