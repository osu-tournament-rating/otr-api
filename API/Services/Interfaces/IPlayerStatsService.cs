using API.DTOs;
using API.Enums;

namespace API.Services.Interfaces;

public interface IPlayerStatsService
{
	Task<PlayerStatsDTO> GetAsync(int playerId, int? comparerId, int mode, DateTime? dateMin = null,
		DateTime? dateMax = null);
	
	Task<PlayerStatsDTO> GetAsync(string username, int? comparerId, int mode, DateTime? dateMin = null,
		DateTime? dateMax = null);

	Task<PlayerTeammateComparisonDTO> GetTeammateComparisonAsync(int playerId, int teammateId, int mode, DateTime dateMin,
		DateTime dateMax);

	Task<PlayerOpponentComparisonDTO> GetOpponentComparisonAsync(int playerId, int opponentId, int mode, DateTime dateMin,
		DateTime dateMax);

	Task<PlayerRankChartDTO> GetRankChartAsync(int playerId, int mode, LeaderboardChartType chartType, DateTime? dateMin = null,
		DateTime? dateMax = null);

	Task BatchInsertAsync(IEnumerable<PlayerMatchStatsDTO> postBody);
	Task BatchInsertAsync(IEnumerable<MatchRatingStatsDTO> postBody);
	Task BatchInsertAsync(IEnumerable<BaseStatsPostDTO> postBody);
	Task BatchInsertAsync(IEnumerable<RatingAdjustmentDTO> postBody);
	Task BatchInsertAsync(IEnumerable<GameWinRecordDTO> postBody);
	Task BatchInsertAsync(IEnumerable<MatchWinRecordDTO> postBody);

	/// <summary>
	///  Truncates both player_match_statistics and match_rating_statistics.
	/// </summary>
	/// <returns></returns>
	Task TruncateAsync();

	Task TruncateRatingAdjustmentsAsync();
}