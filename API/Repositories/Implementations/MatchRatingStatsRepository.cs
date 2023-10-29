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
	                                                                                       .MaxAsync(x => x.GlobalRankAfter);

	public async Task<DateTime?> GetOldestForPlayerAsync(int playerId, int mode) => await _context.MatchRatingStats
	                                                                                       .Where(x => x.PlayerId == playerId && x.Match.Mode == mode)
	                                                                                       .MinAsync(x => x.Match.StartTime); 
}