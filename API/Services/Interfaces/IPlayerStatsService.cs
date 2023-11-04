using API.DTOs;

namespace API.Services.Interfaces;

public interface IPlayerStatsService
{
	Task<PlayerStatsDTO> GetAsync(int playerId, int? comparerId, int mode, DateTime dateMin, DateTime dateMax);
	Task<PlayerTeammateComparisonDTO> GetTeammateComparisonAsync(int playerId, int teammateId, int mode, DateTime dateMin, DateTime dateMax);
	Task<PlayerOpponentComparisonDTO> GetOpponentComparisonAsync(int playerId, int opponentId, int mode, DateTime dateMin, DateTime dateMax);
	Task BatchInsertAsync(IEnumerable<PlayerMatchStatsDTO> postBody);
	Task BatchInsertAsync(IEnumerable<MatchRatingStatsDTO> postBody);
	Task BatchInsertAsync(IEnumerable<BaseStatsPostDTO> postBody);
	/// <summary>
	/// Truncates both player_match_statistics and match_rating_statistics.
	/// </summary>
	/// <returns></returns>
	Task TruncateAsync();
}