using API.DTOs;

namespace API.Services.Interfaces;

public interface IPlayerStatsService
{
	Task<PlayerStatsDTO> GetAsync(long osuPlayerId, int mode, DateTime dateMin, DateTime dateMax);
	Task BatchInsertAsync(IEnumerable<PlayerMatchStatsDTO> postBody);
	Task BatchInsertAsync(IEnumerable<MatchRatingStatsDTO> postBody);
	Task BatchInsertAsync(IEnumerable<BaseStatsPostDTO> postBody);
	/// <summary>
	/// Truncates both player_match_statistics and match_rating_statistics.
	/// </summary>
	/// <returns></returns>
	Task TruncateAsync();
}