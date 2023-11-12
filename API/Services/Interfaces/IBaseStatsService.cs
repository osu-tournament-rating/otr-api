using API.DTOs;
using API.Enums;

namespace API.Services.Interfaces;

public interface IBaseStatsService
{
	/// <summary>
	///  Returns a list of all ratings for a player, one for each game mode (if available)
	/// </summary>
	/// <param name="playerId"></param>
	/// <returns></returns>
	Task<IEnumerable<BaseStatsDTO?>> GetForPlayerAsync(long osuPlayerId);

	Task<BaseStatsDTO?> GetForPlayerAsync(int id, int mode);
	Task<int> BatchInsertAsync(IEnumerable<BaseStatsPostDTO> stats);

	Task<IEnumerable<BaseStatsDTO?>> GetLeaderboardAsync(int mode, int page, int pageSize, LeaderboardChartType chartType,
		LeaderboardFilterDTO filter, int? playerId);

	Task TruncateAsync();
	Task<int> LeaderboardCountAsync(int requestQueryMode, LeaderboardChartType requestQueryChartType, LeaderboardFilterDTO requestQueryFilter, int? playerId);
}