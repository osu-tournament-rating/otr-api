using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class BaseStatsRepository : RepositoryBase<BaseStats>, IBaseStatsRepository
{
	private readonly OtrContext _context;
	private readonly IPlayerRepository _playerRepository;

	public BaseStatsRepository(OtrContext context, IPlayerRepository playerRepository) : base(context)
	{
		_context = context;
		_playerRepository = playerRepository;
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

	public async Task<BaseStats?> GetForPlayerAsync(int playerId, int mode) => await _context.BaseStats.Where(x => x.PlayerId == playerId && x.Mode == mode).FirstOrDefaultAsync();

	public override async Task<int> UpdateAsync(BaseStats entity)
	{
		entity.Updated = DateTime.UtcNow;
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
				Created = DateTime.UtcNow
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

	public async Task<DateTime> GetRecentCreatedDate(long osuPlayerId) =>
		await _context.BaseStats.WhereOsuPlayerId(osuPlayerId).OrderByDescending(x => x.Created).Select(x => x.Created).FirstAsync();

	public async Task<IEnumerable<BaseStats>> GetLeaderboardAsync(int page, int pageSize, int mode, LeaderboardChartType chartType,
		LeaderboardFilterDTO? filter, int? playerId)
	{
		var query = await LeaderboardQuery(mode, chartType, filter, playerId);

		return await query.OrderByRatingDescending()
		                  .Skip(page * pageSize)
		                  .Take(pageSize)
		                  .AsNoTracking()
		                  .ToListAsync();
	}

	public async Task<int> LeaderboardCountAsync(int mode, LeaderboardChartType chartType, LeaderboardFilterDTO filter, int? playerId)
	{
		var query = await LeaderboardQuery(mode, chartType, filter, playerId);

		return await query.CountAsync();
	}

	public async Task<int> HighestRankAsync(int mode, string? country = null)
	{
		if (country != null)
		{
			return await _context.BaseStats
			                     .AsNoTracking()
			                     .Where(x => x.Player.Country == country && x.Mode == mode)
			                     .Select(x => x.CountryRank)
			                     .DefaultIfEmpty()
			                     .MaxAsync();
		}

		return await _context.BaseStats
		                     .AsNoTracking()
		                     .WhereMode(mode)
		                     .Select(x => x.GlobalRank)
		                     .DefaultIfEmpty()
		                     .MaxAsync();
	}

	public async Task<double> HighestRatingAsync(int mode, string? country = null)
	{
		if (country != null)
		{
			return await _context.BaseStats
			                     .AsNoTracking()
			                     .Where(x => x.Player.Country == country && x.Mode == mode)
			                     .Select(x => x.Rating)
			                     .DefaultIfEmpty()
			                     .MaxAsync();
		}

		return await _context.BaseStats
		                     .AsNoTracking()
		                     .Where(x => x.Mode == mode)
		                     .Select(x => x.Rating)
		                     .DefaultIfEmpty()
		                     .MaxAsync();
	}

	public async Task<int> HighestMatchesAsync(int mode, string? country = null)
	{
		if (country != null)
		{
			return await _context.Players
			                     .SelectMany(p => p.MatchStats)
			                     .Where(ms => ms.Match.Mode == mode && ms.Player.Country == country)
			                     .GroupBy(ms => ms.PlayerId)
			                     .OrderByDescending(g => g.Count())
			                     .Select(g => g.Count())
			                     .FirstOrDefaultAsync();
		}

		return await _context.Players
		                     .SelectMany(p => p.MatchStats)
		                     .Where(ms => ms.Match.Mode == mode)
		                     .GroupBy(ms => ms.PlayerId)
		                     .OrderByDescending(g => g.Count())
		                     .Select(g => g.Count())
		                     .FirstOrDefaultAsync();
	}

	public async Task<ActionResult<IEnumerable<double>>> GetHistogramAsync(int mode) =>
		await _context.BaseStats
		               .AsNoTracking()
		               .Where(x => x.Mode == mode)
		               .Select(x => x.Rating)
		               .OrderByDescending(x => x)
		               .ToListAsync();

	public async Task<int> AverageTeammateRating(long osuPlayerId, int mode)
	{
		double averageRating = await _context.MatchScores
		                                     .AsNoTracking()
		                                     .WhereVerified()
		                                     .WhereNotHeadToHead()
		                                     .WhereTeammate(osuPlayerId)
		                                     .SelectMany(x => x.Player.Ratings)
		                                     .Where(rating => rating.Mode == mode)
		                                     .AverageAsync(rating => (double?)rating.Rating) ??
		                       0.0;

		return (int)averageRating;
	}

	private async Task<IQueryable<BaseStats>> LeaderboardQuery(int mode, LeaderboardChartType chartType, LeaderboardFilterDTO? filter, int? playerId)
	{
		var baseQuery = _context.BaseStats
		                        .WhereMode(mode);

		if (chartType == LeaderboardChartType.Country && playerId.HasValue)
		{
			string? playerCountry = await _context.Players.Where(x => x.Id == playerId).Select(x => x.Country).FirstOrDefaultAsync();
			baseQuery = baseQuery.Where(x => x.Player.Country == playerCountry);
		}

		if (filter != null)
		{
			baseQuery = FilterByRank(baseQuery, filter.MinRank, filter.MaxRank, chartType);
			baseQuery = FilterByRating(baseQuery, filter.MinRating, filter.MaxRating);
			baseQuery = FilterByMatchesPlayed(baseQuery, filter.MinMatches, filter.MaxMatches);

			if (filter.TierFilters != null)
			{
				baseQuery = FilterByTier(baseQuery, filter.TierFilters);
			}
			// baseQuery = FilterByWinrate(baseQuery, filter.MinWinrate, filter.MaxWinrate);
		}

		return baseQuery;
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

	private IQueryable<BaseStats> FilterByTier(IQueryable<BaseStats> query, LeaderboardTierFilterDTO tierFilter)
	{
		// Filter for Bronze tier
		if (tierFilter.FilterBronze == true)
		{
			query = query.Where(x => x.Rating < RatingUtils.RatingSilverIII);
		}
		else if (tierFilter.FilterBronze == false)
		{
			query = query.Where(x => x.Rating >= RatingUtils.RatingSilverIII);
		}

		// Filter for Silver tier
		if (tierFilter.FilterSilver == true)
		{
			query = query.Where(x => x.Rating >= RatingUtils.RatingSilverIII && x.Rating < RatingUtils.RatingGoldIII);
		}
		else if (tierFilter.FilterSilver == false)
		{
			query = query.Where(x => x.Rating < RatingUtils.RatingSilverIII || x.Rating >= RatingUtils.RatingGoldIII);
		}

		// Filter for Gold tier
		if (tierFilter.FilterGold == true)
		{
			query = query.Where(x => x.Rating >= RatingUtils.RatingGoldIII && x.Rating < RatingUtils.RatingPlatinumIII);
		}
		else if (tierFilter.FilterGold == false)
		{
			query = query.Where(x => x.Rating < RatingUtils.RatingGoldIII || x.Rating >= RatingUtils.RatingPlatinumIII);
		}

		// Filter for Platinum tier
		if (tierFilter.FilterPlatinum == true)
		{
			query = query.Where(x => x.Rating >= RatingUtils.RatingPlatinumIII && x.Rating < RatingUtils.RatingDiamondIII);
		}
		else if (tierFilter.FilterPlatinum == false)
		{
			query = query.Where(x => x.Rating < RatingUtils.RatingPlatinumIII || x.Rating >= RatingUtils.RatingDiamondIII);
		}

		if (tierFilter.FilterEmerald == true)
		{
			query = query.Where(x => x.Rating >= RatingUtils.RatingEmeraldIII && x.Rating < RatingUtils.RatingMasterIII);
		}
		else if (tierFilter.FilterEmerald == false)
		{
			query = query.Where(x => x.Rating < RatingUtils.RatingEmeraldIII || x.Rating >= RatingUtils.RatingDiamondIII);
		}

		// Filter for Diamond tier
		if (tierFilter.FilterDiamond == true)
		{
			query = query.Where(x => x.Rating >= RatingUtils.RatingDiamondIII && x.Rating < RatingUtils.RatingMasterIII);
		}
		else if (tierFilter.FilterDiamond == false)
		{
			query = query.Where(x => x.Rating < RatingUtils.RatingDiamondIII || x.Rating >= RatingUtils.RatingMasterIII);
		}

		// Filter for Master tier
		if (tierFilter.FilterMaster == true)
		{
			query = query.Where(x => x.Rating >= RatingUtils.RatingMasterIII && x.Rating < RatingUtils.RatingGrandmasterIII);
		}
		else if (tierFilter.FilterMaster == false)
		{
			query = query.Where(x => x.Rating < RatingUtils.RatingMasterIII || x.Rating >= RatingUtils.RatingGrandmasterIII);
		}

		// Filter for Grandmaster tier
		if (tierFilter.FilterGrandmaster == true)
		{
			query = query.Where(x => x.Rating >= RatingUtils.RatingGrandmasterIII && x.Rating < RatingUtils.RatingEliteGrandmaster);
		}
		else if (tierFilter.FilterGrandmaster == false)
		{
			query = query.Where(x => x.Rating < RatingUtils.RatingGrandmasterIII || x.Rating >= RatingUtils.RatingEliteGrandmaster);
		}

		// Filter for Elite Grandmaster tier
		if (tierFilter.FilterEliteGrandmaster == true)
		{
			query = query.Where(x => x.Rating >= RatingUtils.RatingEliteGrandmaster);
		}
		else if (tierFilter.FilterEliteGrandmaster == false)
		{
			query = query.Where(x => x.Rating < RatingUtils.RatingEliteGrandmaster);
		}

		return query;
	}
}