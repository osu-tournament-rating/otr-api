using API.Entities;

namespace API.Services.Interfaces;

public interface IMatchDataService : IService<MatchData>
{
	/// <summary>
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<MatchData>> GetFilteredDataAsync();

	Task<IEnumerable<MatchData>> GetAllForPlayerAsync(int playerId);
	Task<IEnumerable<MatchData>> GetAllForOsuMatchIdAsync(long osuMatchId);
	Task<int> GetIdForPlayerIdGameIdAsync(int playerId, long gameId);
	Task<IEnumerable<(int id, int playerId, long gameId)>> GetIdsPlayerIdsGameIdsAsync();
	Task<int?> GetIdAsync(int playerId, long osuMatchId, long gameId);
	Task<IEnumerable<int>> GetIdsAsync(IEnumerable<int> playerIds, IEnumerable<long> osuMatchIds, IEnumerable<long> gameIds);
}