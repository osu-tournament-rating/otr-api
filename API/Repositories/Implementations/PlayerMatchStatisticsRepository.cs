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
		                     .Include(stats => stats.Player)
		                     .Include(stats => stats.Match)
		                     .Where(stats => stats.PlayerId == playerId &&
		                                     stats.Match.Tournament != null && stats.Match.Tournament.Mode == mode &&
		                                     stats.Match.StartTime >= dateMin &&
		                                     stats.Match.StartTime <= dateMax)
		                     .OrderBy(x => x.Match.StartTime)
		                     .ToListAsync();
	}

	public async Task InsertAsync(PlayerMatchStatistics postBody)
	{
		await _context.PlayerMatchStatistics.AddAsync(postBody);
		await _context.SaveChangesAsync();
	}

	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE player_match_statistics RESTART IDENTITY;"); 
}