using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class MatchRatingStatisticsRepository : IMatchRatingStatisticsRepository
{
	private readonly OtrContext _context;

	public MatchRatingStatisticsRepository(OtrContext context) { _context = context; }
	
	public async Task<IEnumerable<MatchRatingStatistics>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		return await _context.MatchRatingStatistics
		                     .Where(x => x.PlayerId == playerId && x.Match.Mode == mode && x.Match.StartTime >= dateMin && x.Match.StartTime <= dateMax)
		                     .ToListAsync();
	}

	public async Task InsertAsync(MatchRatingStatistics postBody)
	{
		await _context.MatchRatingStatistics.AddAsync(postBody);
		await _context.SaveChangesAsync();
	}

	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE match_rating_statistics RESTART IDENTITY"); 
}