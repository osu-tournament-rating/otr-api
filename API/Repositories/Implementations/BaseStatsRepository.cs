using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class BaseStatsRepository : RepositoryBase<BaseStats>, IBaseStatsRepository
{
	private readonly OtrContext _context;
	private readonly IPlayerRepository _playerRepository;
	
	
	private readonly IPlayerMatchStatsRepository _matchStatsRepository;

	public BaseStatsRepository(OtrContext context, IPlayerRepository playerRepository, IPlayerMatchStatsRepository matchStatsRepository) : base(context)
	{
		_context = context;
		_playerRepository = playerRepository;
		_matchStatsRepository = matchStatsRepository;
	}

	public async Task<IEnumerable<BaseStats>> GetForPlayerAsync(long osuPlayerId)
	{
		int dbId = await _playerRepository.GetIdByOsuIdAsync(osuPlayerId);

		if (dbId == default)
		{
			return new List<BaseStats>();
		}

		return await _context.BaseStats.Where(x => x.PlayerId == dbId).ToListAsync();
	}

	public async Task<BaseStats?> GetForPlayerAsync(int playerId, int mode)
	{
		return await _context.BaseStats.Where(x => x.PlayerId == playerId && x.Mode == mode).FirstOrDefaultAsync();
	} 

	public override async Task<int> UpdateAsync(BaseStats entity)
	{
		// First, copy the current state of the entity to the history table.
		var history = new RatingHistory
		{
			PlayerId = entity.PlayerId,
			Mu = entity.Rating,
			Sigma = entity.Volatility,
			Created = DateTime.UtcNow,
			Mode = entity.Mode
		};

		await _context.RatingHistories.AddAsync(history);
		return await base.UpdateAsync(entity);
	}

	public async Task<int> InsertOrUpdateForPlayerAsync(int playerId, BaseStats baseStats)
	{
		var existingRating = await _context.BaseStats
		                                   .Where(r => r.PlayerId == baseStats.PlayerId && r.Mode == baseStats.Mode)
		                                   .FirstOrDefaultAsync();

		if (existingRating != null)
		{
			existingRating.Rating = baseStats.Rating;
			existingRating.Volatility = baseStats.Volatility;
			existingRating.Updated = baseStats.Updated;
		}
		else
		{
			_context.BaseStats.Add(baseStats);
		}

		return await _context.SaveChangesAsync();
	}

	public async Task<int> BatchInsertAsync(IEnumerable<BaseStats> baseStats)
	{
		var ls = new List<BaseStats>();
		foreach (var stat in baseStats)
		{
			ls.Add(new BaseStats
			{
				PlayerId = stat.PlayerId,
				MatchCostAverage = stat.MatchCostAverage,
				Mode = stat.Mode,
				Rating = stat.Rating,
				Volatility = stat.Volatility,
				Percentile = stat.Percentile,
				GlobalRank = stat.GlobalRank,
				CountryRank = stat.CountryRank,
				Created = DateTime.UtcNow,
			});
		}

		await _context.BaseStats.AddRangeAsync(ls);
		return await _context.SaveChangesAsync();
	}

	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE base_stats RESTART IDENTITY;");

	public async Task<int> GetGlobalRankAsync(long osuPlayerId, int mode)
	{
		int globalIndex = (await _context.BaseStats
		                          .WhereMode(mode)
		                          .OrderByRatingDescending()
		                          .Select(x => x.Player.OsuId)
		                          .ToListAsync())
		                          .TakeWhile(x => x != osuPlayerId)
		                          .Count();

		return globalIndex + 1;
	}

	public async Task<int> AverageTeammateRating(long osuPlayerId, int mode)
	{
		double averageRating = await _context.MatchScores
		                                     .WhereVerified()
		                                     .WhereNotHeadToHead()
		                                     .WhereTeammate(osuPlayerId)
		                                     .SelectMany(x => x.Player.Ratings)
		                                     .Where(rating => rating.Mode == mode)
		                                     .AverageAsync(rating => (double?)rating.Rating) ?? 0.0;

		return (int)averageRating;
	}

	public async Task<DateTime> GetRecentCreatedDate(long osuPlayerId) =>
		await _context.BaseStats.WhereOsuPlayerId(osuPlayerId).OrderByDescending(x => x.Created).Select(x => x.Created).FirstAsync();


	public async Task<IEnumerable<BaseStats>> GetLeaderboardAsync(int page, int pageSize, int mode, LeaderboardChartType chartType, LeaderboardFilterDTO? filter)
	{
		var baseQuery = _context.BaseStats
		                        .WhereMode(mode);


		if (filter != null)
		{
			baseQuery = FilterByRank(baseQuery, filter.MinRank, filter.MaxRank, chartType);
			baseQuery = FilterByRating(baseQuery, filter.MinRating, filter.MaxRating);
			baseQuery = FilterByMatchesPlayed(baseQuery, filter.MinMatches, filter.MaxMatches);
			// baseQuery = FilterByWinrate(baseQuery, filter.MinWinrate, filter.MaxWinrate);
		}
		
		return await baseQuery.OrderByRatingDescending()
		                .Skip(page * pageSize)
		                .Take(pageSize)
		                .ToListAsync();

	}
	
	private IQueryable<BaseStats> FilterByRank(IQueryable<BaseStats> query, int? minRank, int? maxRank, LeaderboardChartType chartType)
	{
		if (minRank.HasValue)
		{
			if (chartType == LeaderboardChartType.Country)
			{
				query = query.Where(x => x.CountryRank >= minRank.Value);
			}
			else
			{
				query = query.Where(x => x.GlobalRank >= minRank.Value);
			}
		}

		if (maxRank.HasValue)
		{
			if (chartType == LeaderboardChartType.Country)
			{
				query = query.Where(x => x.CountryRank <= maxRank.Value);
			}
			else
			{
				query = query.Where(x => x.GlobalRank <= maxRank.Value);
			}
		}

		return query;
	}
	
	private IQueryable<BaseStats> FilterByRating(IQueryable<BaseStats> query, int? minRating, int? maxRating)
	{
		if (minRating.HasValue)
		{
			query = query.Where(x => x.Rating >= minRating.Value);
		}

		if (maxRating.HasValue)
		{
			query = query.Where(x => x.Rating <= maxRating.Value);
		}

		return query;
	}
	
	private IQueryable<BaseStats> FilterByMatchesPlayed(IQueryable<BaseStats> query, int? minMatches, int? maxMatches)
	{
		// This is required to count the number of matches played.
		// In the future this should be a stat tied to BaseStats, not calculated.
		
		if (minMatches.HasValue || maxMatches.HasValue)
		{
			query = query.Include(x => x.Player).ThenInclude(x => x.MatchStats);
		}
		
		if (minMatches.HasValue)
		{
			
			query = query.Where(x => x.Player.MatchStats.Count() >= minMatches.Value);
		}

		if (maxMatches.HasValue)
		{
			query = query.Where(x => x.Player.MatchStats.Count() <= maxMatches.Value);
		}

		return query;
	}
	
	// TODO: Insert winrate as a base stats before implementing
	// private IQueryable<BaseStats> FilterByWinrate(IQueryable<BaseStats> query, double? minWinrate, double? maxWinrate)
	// {
	// 	if(minWinrate.HasValue || maxWinrate.HasValue)
	// 	{
	// 		query = query.Include(x => x.Player).ThenInclude(x => x.MatchStats);
	// 	}
	// 	
	// 	if (minWinrate.HasValue)
	// 	{
	// 		query = query.Where(x => x.Player. >= minWinrate.Value);
	// 	}
	//
	// 	if (maxWinrate.HasValue)
	// 	{
	// 		query = query.Where(x => x.Player.MatchStats.Count() <= maxWinrate.Value);
	// 	}
	//
	// 	return query;
	// }
}