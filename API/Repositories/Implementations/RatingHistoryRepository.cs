using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class RatingHistoryRepository : RepositoryBase<RatingHistory>, IRatingHistoryRepository
{
	private readonly OtrContext _context;
	public RatingHistoryRepository(OtrContext context) : base(context) { _context = context; }

	public async Task<IEnumerable<RatingHistory>> GetForPlayerAsync(long osuPlayerId, int mode, DateTime fromTime) => await _context.RatingHistories
	                                                                                                                                .Include(x => x.Match)
	                                                                                                                                .WhereOsuPlayerId(osuPlayerId)
	                                                                                                                                .WhereMode(mode)
	                                                                                                                                .After(fromTime)
	                                                                                                                                .OrderBy(x => x.Created)
	                                                                                                                                .ToListAsync();

	public async Task<IEnumerable<RatingHistory>> GetForPlayerAsync(long osuPlayerId) => await _context.RatingHistories
	                                                                                                   .Include(x => x.Match)
	                                                                                                   .WhereOsuPlayerId(osuPlayerId)
	                                                                                                   .ToListAsync();

	public async Task<int> BatchInsertAsync(IEnumerable<RatingHistoryDTO> histories)
	{
		var ls = new List<RatingHistory>();
		foreach (var history in histories)
		{
			ls.Add(new RatingHistory
			{
				PlayerId = history.PlayerId,
				Mu = history.Mu,
				Sigma = history.Sigma,
				Created = history.Created,
				Mode = history.Mode,
				MatchId = history.MatchId
			});
		}

		await _context.RatingHistories.AddRangeAsync(ls);
		return await _context.SaveChangesAsync();
	}

	public async Task<RatingHistory?> GetOldestForPlayerAsync(long osuId, int mode) =>
		await _context.RatingHistories.WhereOsuPlayerId(osuId).WhereMode(mode).OrderBy(x => x.Created).FirstOrDefaultAsync();

	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ratinghistories RESTART IDENTITY");
}