using API.Entities;

namespace API.Services.Interfaces;

public interface IMatchDataService : IService<PlayerMatchData>
{
	/// <summary>
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<PlayerMatchData>> GetFilteredDataAsync();

	Task<IEnumerable<PlayerMatchData>> GetAllForPlayerAsync(int playerId);
	Task<IEnumerable<PlayerMatchData>> GetAllForOsuMatchIdAsync(long osuMatchId);
	Task<int> GetIdForPlayerIdGameIdAsync(int playerId, long gameId);
	Task<IEnumerable<(int id, int playerId, long gameId)>> GetIdsPlayerIdsGameIdsAsync();
	Task<int?> GetIdAsync(int playerId, long osuMatchId, long gameId);
	Task<Dictionary<(int, long), int>> GetIdsAsync(IEnumerable<int> playerIds, IEnumerable<long> osuMatchIds, IEnumerable<long> gameIds);
}