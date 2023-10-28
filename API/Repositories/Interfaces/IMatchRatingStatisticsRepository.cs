using API.Entities;

namespace API.Repositories.Interfaces;

public interface IMatchRatingStatisticsRepository
{
	Task<IEnumerable<MatchRatingStatistics>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);
	Task InsertAsync(MatchRatingStatistics item);
	Task InsertAsync(IEnumerable<MatchRatingStatistics> items);
	Task TruncateAsync();
}