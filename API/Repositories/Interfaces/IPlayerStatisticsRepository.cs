using API.Entities;

namespace API.Repositories.Interfaces;

public interface IPlayerStatisticsRepository : IRepository<PlayerStatistics>
{
	Task<PlayerStatistics?> GetForPlayerAsync(int playerId, DateTime dateMin, DateTime dateMax);
}