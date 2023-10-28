using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IPlayerStatisticsService
{
	Task<PlayerStatisticsDTO> GetAsync(long osuPlayerId, int mode, DateTime dateMin, DateTime dateMax);
	Task BatchInsertAsync(IEnumerable<PlayerMatchStatisticsDTO> postBody);
	Task BatchInsertAsync(IEnumerable<MatchRatingStatisticsDTO> postBody);
	/// <summary>
	/// Truncates both player_match_statistics and match_rating_statistics.
	/// </summary>
	/// <returns></returns>
	Task TruncateAsync();
}