using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class PlayerMatchStatsRepository : IPlayerMatchStatsRepository
{
	private readonly OtrContext _context;

	public PlayerMatchStatsRepository(OtrContext context) { _context = context; }

	public async Task<IEnumerable<PlayerMatchStats>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		return await _context.PlayerMatchStats
		                     .Where(stats => stats.PlayerId == playerId &&
		                                     stats.Match.Mode == mode &&
		                                     stats.Match.StartTime >= dateMin &&
		                                     stats.Match.StartTime <= dateMax)
		                     .OrderBy(x => x.Match.StartTime)
		                     .ToListAsync();
	}

	public async Task<bool> WonAsync(int playerId, int matchId)
	{
		return await _context.PlayerMatchStats
		                     .Where(stats => stats.PlayerId == playerId && stats.MatchId == matchId)
		                     .Select(stats => stats.Won)
		                     .FirstOrDefaultAsync();
	}

	public async Task InsertAsync(PlayerMatchStats item)
	{
		await _context.PlayerMatchStats.AddAsync(item);
		await _context.SaveChangesAsync();
	}

	public async Task InsertAsync(IEnumerable<PlayerMatchStats> items)
	{
		await _context.PlayerMatchStats.AddRangeAsync(items);
		await _context.SaveChangesAsync();
	}

	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE player_match_statistics RESTART IDENTITY;");

	public async Task<int> CountMatchesPlayedAsync(int playerId, int mode) =>
		await _context.PlayerMatchStats.Where(x => x.PlayerId == playerId && x.Match.Mode == mode).CountAsync();

	public async Task<double> WinRateAsync(int playerId, int mode)
	{
		int matchesPlayed = await CountMatchesPlayedAsync(playerId, mode);
		int matchesWon = await _context.PlayerMatchStats.Where(x => x.PlayerId == playerId && x.Match.Mode == mode && x.Won).CountAsync();
		
		if (matchesPlayed == 0)
		{
			return 0;
		}
		
		return matchesWon / (double) matchesPlayed;
	}
}