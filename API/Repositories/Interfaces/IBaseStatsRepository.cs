using API.DTOs;
using API.Entities;
using API.Enums;

namespace API.Repositories.Interfaces;

public interface IBaseStatsRepository : IRepository<BaseStats>
{
	/// <summary>
	///  Returns all ratings for a player, one for each game mode (if available)
	/// </summary>
	/// <param name="playerId"></param>
	/// <returns></returns>
	Task<IEnumerable<BaseStats>> GetForPlayerAsync(long osuPlayerId);

	Task<BaseStats?> GetForPlayerAsync(int playerId, int mode);

	Task<int> InsertOrUpdateForPlayerAsync(int playerId, BaseStats baseStats);
	Task<int> BatchInsertAsync(IEnumerable<BaseStats> baseStats);
	Task<IEnumerable<BaseStats>> GetAllAsync();
	Task TruncateAsync();
	Task<int> GetGlobalRankAsync(long osuPlayerId, int mode);
	/// <summary>
	/// Returns the creation date of the most recently created rating entry for a player
	/// </summary>
	/// <returns></returns>
	Task<DateTime> GetRecentCreatedDate(long osuPlayerId);
	Task<IEnumerable<BaseStats>> GetLeaderboardAsync(int page, int pageSize, int mode, LeaderboardChartType chartType, LeaderboardFilterDTO? filter);
}