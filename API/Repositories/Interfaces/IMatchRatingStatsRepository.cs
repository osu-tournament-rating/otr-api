using API.DTOs;
using API.Entities;
using API.Enums;

namespace API.Repositories.Interfaces;

public interface IMatchRatingStatsRepository
{
	Task<IEnumerable<MatchRatingStats>> GetForPlayerAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null);
	Task InsertAsync(MatchRatingStats item);
	Task InsertAsync(IEnumerable<MatchRatingStats> items);
	Task TruncateAsync();
	Task<int> HighestGlobalRankAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null);
	Task<int> HighestCountryRankAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null);
	Task<DateTime?> GetOldestForPlayerAsync(int playerId, int mode);
	Task<IEnumerable<MatchRatingStats>> TeammateRatingStatsAsync(int playerId, int teammateId, int mode, DateTime dateMin, DateTime dateMax);
	Task<IEnumerable<MatchRatingStats>> OpponentRatingStatsAsync(int playerId, int opponentId, int mode, DateTime dateMin, DateTime dateMax);
	Task<PlayerRankChartDTO> GetRankChartAsync(int playerId, int mode, LeaderboardChartType chartType, DateTime? dateMin = null, DateTime? dateMax = null);
}