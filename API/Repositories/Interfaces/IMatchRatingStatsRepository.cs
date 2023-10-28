using API.Entities;

namespace API.Repositories.Interfaces;

public interface IMatchRatingStatsRepository
{
	Task<IEnumerable<MatchRatingStats>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);
	Task InsertAsync(MatchRatingStats item);
	Task InsertAsync(IEnumerable<MatchRatingStats> items);
	Task TruncateAsync();
	Task<int> HighestGlobalRankAsync(int playerId, int mode);
	Task<DateTime?> GetOldestForPlayerAsync(int playerId, int mode);
}