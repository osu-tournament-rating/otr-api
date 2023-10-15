using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class PlayerGameStatisticsRepository : RepositoryBase<PlayerGameStatistics>, IPlayerGameStatisticsRepository
{
	private readonly OtrContext _context;
	public PlayerGameStatisticsRepository(OtrContext context) : base(context) { _context = context; }

	public async Task<IEnumerable<PlayerGameStatistics>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		return await _context.PlayerGameStatistics
		                     .Include(stats => stats.Player)
		                     .Include(stats => stats.Game)
		                     .Where(stats => stats.PlayerId == playerId &&
		                                     stats.Mode == mode &&
		                                     stats.Game.StartTime >= dateMin &&
		                                     stats.Game.StartTime <= dateMax)
		                     .ToListAsync();
	}
}