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
	public MatchDataService(ICredentials credentials, ILogger<MatchDataService> logger) : base(credentials, logger) {}

	public async Task<IEnumerable<MatchData>> GetFilteredDataAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<MatchData>(_filterQuery);
		}
	}

	public async Task<IEnumerable<MatchData>> GetAllForPlayerAsync(int playerId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<MatchData>("SELECT * FROM matchdata WHERE player_id = @PlayerId", new { PlayerId = playerId });
		}
	}

	public Task<IEnumerable<MatchData>> GetAllForOsuMatchIdAsync(long osuMatchId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return connection.QueryAsync<MatchData>("SELECT * FROM matchdata WHERE osu_match_id = @OsuMatchId", new { OsuMatchId = osuMatchId });
		}
	}

	public async Task<int> GetIdForPlayerIdGameIdAsync(int playerId, long gameId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleAsync<int>("SELECT id FROM matchdata WHERE player_id = @PlayerId AND game_id = @GameId",
				new { PlayerId = playerId, GameId = gameId });
		}
	}

	public async Task<IEnumerable<(int id, int playerId, long gameId)>> GetIdsPlayerIdsGameIdsAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<(int id, int playerId, long gameId)>("SELECT id, player_id, game_id FROM matchdata");
		}
	}
}