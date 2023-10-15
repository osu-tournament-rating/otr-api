using API.Entities;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations;

public class PlayerStatisticsRepository : RepositoryBase<PlayerStatistics>, IPlayerStatisticsRepository
{
	private readonly OtrContext _context;
	public PlayerStatisticsRepository(OtrContext context) : base(context) { _context = context; }

	public async Task<PlayerStatistics?> GetForPlayerAsync(int playerId, DateTime dateMin, DateTime dateMax)
	{
		throw new NotImplementedException();
	}
}