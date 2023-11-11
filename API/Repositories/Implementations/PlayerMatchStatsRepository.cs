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

	public async Task<IEnumerable<PlayerMatchStats>> TeammateStatsAsync(int playerId, int teammateId, int mode, DateTime dateMin,
		DateTime dateMax)
	{
		return await _context.PlayerMatchStats
		                     .Where(stats => stats.PlayerId == playerId && stats.TeammateIds.Contains(teammateId) && stats.Match.Mode == mode &&
		                                     stats.Match.StartTime >= dateMin && stats.Match.StartTime <= dateMax)
		                     .OrderBy(x => x.Match.StartTime)
		                     .ToListAsync();
	}

	public async Task<IEnumerable<PlayerMatchStats>> OpponentStatsAsync(int playerId, int opponentId, int mode, DateTime dateMin,
		DateTime dateMax)
	{ 
		return await _context.PlayerMatchStats
		                     .Where(stats => stats.PlayerId == playerId && stats.OpponentIds.Contains(opponentId) && stats.Match.Mode == mode &&
		                                     stats.Match.StartTime >= dateMin && stats.Match.StartTime <= dateMax)
		                     .OrderBy(x => x.Match.StartTime)
		                     .ToListAsync();
	}

	public async Task InsertAsync(IEnumerable<PlayerMatchStats> items)
	{
		await _context.PlayerMatchStats.AddRangeAsync(items);
		await _context.SaveChangesAsync();
	}

	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE player_match_stats RESTART IDENTITY;");

	public async Task<int> CountMatchesPlayedAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null)
	{
		dateMin ??= DateTime.MinValue;
		dateMax ??= DateTime.MaxValue;
		
		return await _context.PlayerMatchStats.Where(x => x.PlayerId == playerId && x.Match.Mode == mode && 
		                                                  x.Match.StartTime >= dateMin && x.Match.StartTime <= dateMax).CountAsync();
	}
	
	public async Task<int> CountMatchesWonAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null)
	{
		dateMin ??= DateTime.MinValue;
		dateMax ??= DateTime.MaxValue;
		
		return await _context.PlayerMatchStats.Where(x => x.PlayerId == playerId && x.Match.Mode == mode && x.Won && 
		                                                  x.Match.StartTime >= dateMin && x.Match.StartTime <= dateMax).CountAsync();
	}

	public async Task<double> GlobalWinrateAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null)
	{
		dateMin ??= DateTime.MinValue;
		dateMax ??= DateTime.MaxValue;
		
		int matchesPlayed = await CountMatchesPlayedAsync(playerId, mode, dateMin, dateMax);
		int matchesWon = await CountMatchesWonAsync(playerId, mode, dateMin, dateMax);
		
		if (matchesPlayed == 0)
		{
			return 0;
		}
		
		return matchesWon / (double) matchesPlayed;
	}
}