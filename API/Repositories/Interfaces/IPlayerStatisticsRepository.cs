using API.Entities;

namespace API.Repositories.Interfaces;

public interface IPlayerGameStatisticsRepository : IRepository<PlayerGameStatistics>
{
	Task<IEnumerable<PlayerGameStatistics>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);
}