using API.Entities;

namespace API.Repositories.Interfaces;

public interface IPlayerMatchStatisticsRepository
{
	Task<IEnumerable<PlayerMatchStatistics>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);
	Task InsertAsync(PlayerMatchStatistics postBody);
	Task TruncateAsync();
}