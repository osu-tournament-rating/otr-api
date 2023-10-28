using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class PlayerMatchStatisticsRepository : IPlayerMatchStatisticsRepository
{
	private readonly OtrContext _context;

	public PlayerMatchStatisticsRepository(OtrContext context) { _context = context; }
	
	public async Task<IEnumerable<PlayerMatchStatistics>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		return await _context.PlayerMatchStatistics
		                     .Where(stats => stats.PlayerId == playerId && 
		                                     stats.Match.Mode == mode &&
		                                     stats.Match.StartTime >= dateMin &&
		                                     stats.Match.StartTime <= dateMax)
		                     .OrderBy(x => x.Match.StartTime)
		                     .ToListAsync();
	}

	public async Task InsertAsync(PlayerMatchStatistics item)
	{
		await _context.PlayerMatchStatistics.AddAsync(item);
		await _context.SaveChangesAsync();
	}

	public async Task InsertAsync(IEnumerable<PlayerMatchStatistics> items)
	{
		await _context.PlayerMatchStatistics.AddRangeAsync(items);
		await _context.SaveChangesAsync();
	} 
	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE player_match_statistics RESTART IDENTITY;"); 
}