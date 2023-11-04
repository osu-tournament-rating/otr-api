using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class MatchRatingStatsRepository : IMatchRatingStatsRepository
{
	private readonly OtrContext _context;

	public MatchRatingStatsRepository(OtrContext context) { _context = context; }
	
	public async Task<IEnumerable<MatchRatingStats>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		return await _context.MatchRatingStats
		                     .Where(x => x.PlayerId == playerId && 
		                                 x.Match.Mode == mode && 
		                                 x.Match.StartTime >= dateMin && 
		                                 x.Match.StartTime <= dateMax)
		                     .Include(x => x.Match)
		                     .ThenInclude(x => x.Tournament)
		                     .ToListAsync();
	}

	public async Task InsertAsync(MatchRatingStats item)
	{
		await _context.MatchRatingStats.AddAsync(item);
		await _context.SaveChangesAsync();
	}

	public async Task InsertAsync(IEnumerable<MatchRatingStats> items)
	{
		await _context.MatchRatingStats.AddRangeAsync(items);
		await _context.SaveChangesAsync();
	}
	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE match_rating_stats RESTART IDENTITY");
	public async Task<int> HighestGlobalRankAsync(int playerId, int mode) => await _context.MatchRatingStats
	                                                                                       .Where(x => x.PlayerId == playerId && x.Match.Mode == mode)
	                                                                                       .Select(x => x.GlobalRankAfter)
	                                                                                       .DefaultIfEmpty()
	                                                                                       .MaxAsync();

	public async Task<DateTime?> GetOldestForPlayerAsync(int playerId, int mode) => await _context.MatchRatingStats
	                                                                                       .Where(x => x.PlayerId == playerId && x.Match.Mode == mode && x.Match.StartTime != null)
	                                                                                       .Select(x => x.Match.StartTime)
	                                                                                       .MinAsync();

	public async Task<IEnumerable<MatchRatingStats>> TeammateRatingStatsAsync(int playerId, int teammateId, int mode, DateTime dateMin, DateTime dateMax)
	{
		return await _context.MatchRatingStats
		                     .Where(mrs => mrs.PlayerId == playerId)
		                     .Where(mrs => _context.PlayerMatchStats
		                                           .Any(pms => pms.PlayerId == mrs.PlayerId && pms.TeammateIds.Contains(teammateId) && pms.Match.Mode == mode && 
		                                                       pms.Match.StartTime >= dateMin && pms.Match.StartTime <= dateMax))
		                     .Distinct()
		                     .ToListAsync();
	}

	public async Task<IEnumerable<MatchRatingStats>> OpponentRatingStatsAsync(int playerId, int opponentId, int mode, DateTime dateMin, DateTime dateMax)
	{
		return await _context.MatchRatingStats
		                     .Where(mrs => mrs.PlayerId == playerId)
		                     .Where(mrs => _context.PlayerMatchStats
		                                           .Any(pms => pms.PlayerId == mrs.PlayerId && pms.OpponentIds.Contains(opponentId) && pms.Match.Mode == mode && 
		                                                       pms.Match.StartTime >= dateMin && pms.Match.StartTime <= dateMax))
		                     .Distinct()
		                     .ToListAsync();
	}
}