using API.Entities;

namespace API.Repositories.Interfaces;

public interface IPlayerMatchStatisticsRepository
{
	Task<IEnumerable<PlayerMatchStatistics>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);
	Task InsertAsync(PlayerMatchStatistics item);
	Task InsertAsync(IEnumerable<PlayerMatchStatistics> items);
	Task TruncateAsync();
}