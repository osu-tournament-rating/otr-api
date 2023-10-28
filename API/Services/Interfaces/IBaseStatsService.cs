using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IBaseStatsService
{
	/// <summary>
	///  Returns a list of all ratings for a player, one for each game mode (if available)
	/// </summary>
	/// <param name="playerId"></param>
	/// <returns></returns>
	Task<IEnumerable<BaseStatsDTO>> GetForPlayerAsync(long osuPlayerId);
	Task<BaseStatsDTO?> GetForPlayerAsync(int id, int mode);
	Task<int> BatchInsertAsync(IEnumerable<BaseStatsPostDTO> stats);
	Task<IEnumerable<BaseStatsDTO>> GetAllAsync();
	Task TruncateAsync();
	/// <summary>
	/// Returns the creation date of the most recently created rating entry for a player
	/// </summary>
	/// <returns></returns>
	Task<DateTime> GetRecentCreatedDate(long osuPlayerId);
	Task<int?> InsertOrUpdateAsync(int playerId, BaseStats baseStats);
}