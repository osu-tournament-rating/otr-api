using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Entities;
using API.Enums;
using API.Handlers.Interfaces;
using API.Osu.Enums;
using API.Repositories.Interfaces;
using API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class BaseStatsRepository(OtrContext context, IPlayerRepository playerRepository, ICacheHandler cacheHandler) : RepositoryBase<BaseStats>(context), IBaseStatsRepository, IUsesCache
{
    private readonly OtrContext _context = context;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly ICacheHandler _cacheHandler = cacheHandler;

    public async Task InvalidateCacheEntriesAsync()
    {
        await _cacheHandler.OnBaseStatsUpdateAsync();
    }

    public async Task<IEnumerable<BaseStats>> GetForPlayerAsync(long osuPlayerId)
    {
        var id = await _playerRepository.GetIdAsync(osuPlayerId);

        if (!id.HasValue)
        {
            return new List<BaseStats>();
        }

        return await _context.BaseStats.Where(x => x.PlayerId == id.Value).ToListAsync();
    }

    public async Task<BaseStats?> GetForPlayerAsync(int playerId, int mode) =>
        await _context.BaseStats.Where(x => x.PlayerId == playerId && x.Mode == (Ruleset)mode).FirstOrDefaultAsync();

    public override async Task<int> UpdateAsync(BaseStats entity)
    {
        entity.Updated = DateTime.UtcNow;
        return await base.UpdateAsync(entity);
    }

    public async Task<int> InsertOrUpdateForPlayerAsync(int playerId, BaseStats baseStats)
    {
        BaseStats? existingRating = await _context
            .BaseStats.Where(r => r.PlayerId == baseStats.PlayerId && r.Mode == baseStats.Mode)
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
        foreach (BaseStats stat in baseStats)
        {
            ls.Add(
                new BaseStats
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
                }
            );
        }

        await _context.BaseStats.AddRangeAsync(ls);
        return await _context.SaveChangesAsync();
    }

    public async Task TruncateAsync() =>
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE base_stats RESTART IDENTITY;");

    public async Task<int> GetGlobalRankAsync(long osuPlayerId, int mode)
    {
        var globalIndex = (
            await _context
                .BaseStats.WhereRuleset((Ruleset)mode)
                .OrderByRatingDescending()
                .Select(x => x.Player.OsuId)
                .ToListAsync()
        )
            .TakeWhile(x => x != osuPlayerId)
            .Count();

        return globalIndex + 1;
    }

    public async Task<DateTime> GetRecentCreatedDate(long osuPlayerId) =>
        await _context
            .BaseStats.WhereOsuPlayerId(osuPlayerId)
            .OrderByDescending(x => x.Created)
            .Select(x => x.Created)
            .FirstAsync();

    public async Task<IEnumerable<BaseStats>> GetLeaderboardAsync(
        int page,
        int pageSize,
        int mode,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO? filter,
        int? playerId
    )
    {
        IQueryable<BaseStats> query = await LeaderboardQuery(mode, chartType, filter, playerId);

        return await query
            .OrderByRatingDescending()
            .Skip(page * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> LeaderboardCountAsync(
        int mode,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO filter,
        int? playerId
    )
    {
        IQueryable<BaseStats> query = await LeaderboardQuery(mode, chartType, filter, playerId);

        return await query.CountAsync();
    }

    public async Task<int> HighestRankAsync(int mode, string? country = null)
    {
        if (country != null)
        {
            return await _context
                .BaseStats.AsNoTracking()
                .Where(x => x.Player.Country == country && x.Mode == (Ruleset)mode)
                .Select(x => x.CountryRank)
                .DefaultIfEmpty()
                .MaxAsync();
        }

        return await _context
            .BaseStats.AsNoTracking()
            .WhereRuleset((Ruleset)mode)
            .Select(x => x.GlobalRank)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public async Task<double> HighestRatingAsync(int mode, string? country = null)
    {
        if (country != null)
        {
            return await _context
                .BaseStats.AsNoTracking()
                .Where(x => x.Player.Country == country && x.Mode == (Ruleset)mode)
                .Select(x => x.Rating)
                .DefaultIfEmpty()
                .MaxAsync();
        }

        return await _context
            .BaseStats.AsNoTracking()
            .Where(x => x.Mode == (Ruleset)mode)
            .Select(x => x.Rating)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public async Task<int> HighestMatchesAsync(int mode, string? country = null)
    {
        if (country != null)
        {
            return await _context
                .Players.SelectMany(p => p.MatchStats)
                .Where(ms => ms.Match.Tournament.Mode == mode && ms.Player.Country == country)
                .GroupBy(ms => ms.PlayerId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Count())
                .FirstOrDefaultAsync();
        }

        return await _context
            .Players.SelectMany(p => p.MatchStats)
            .Where(ms => ms.Match.Tournament.Mode == mode)
            .GroupBy(ms => ms.PlayerId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Count())
            .FirstOrDefaultAsync();
    }

    public async Task<IDictionary<int, int>> GetHistogramAsync(int mode)
    {
        // Determine the maximum rating as a double
        var maxRating = await _context.BaseStats.Where(x => x.Mode == (Ruleset)mode).MaxAsync(x => x.Rating);

        // Round up maxRating to the nearest multiple of 25
        var maxBucket = (int)(Math.Ceiling(maxRating / 25) * 25);

        // Initialize all buckets from 100 to maxBucket with 0
        var histogram = Enumerable.Range(4, (maxBucket / 25) - 3).ToDictionary(bucket => bucket * 25, _ => 0);

        // Adjust the GroupBy to correctly bucket the rating of 100
        Dictionary<int, int> dbHistogram = await _context
            .BaseStats.AsNoTracking()
            .Where(x => x.Mode == (Ruleset)mode && x.Rating >= 100)
            .GroupBy(x => (int)(x.Rating / 25) * 25)
            .Select(g => new { Bucket = g.Key == 0 ? 100 : g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Bucket, g => g.Count);

        // Update the histogram with actual counts
        foreach (KeyValuePair<int, int> item in dbHistogram)
        {
            if (histogram.ContainsKey(item.Key))
            {
                histogram[item.Key] = item.Value;
            }
        }

        return histogram;
    }

    public async Task<int> AverageTeammateRating(long osuPlayerId, int mode)
    {
        var averageRating =
            await _context
                .MatchScores.AsNoTracking()
                .WhereVerified()
                .WhereNotHeadToHead()
                .WhereTeammate(osuPlayerId)
                .SelectMany(x => x.Player.Ratings)
                .Where(rating => rating.Mode == (Ruleset)mode)
                .AverageAsync(rating => (double?)rating.Rating) ?? 0.0;

        return (int)averageRating;
    }

    private async Task<IQueryable<BaseStats>> LeaderboardQuery(
        int mode,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO? filter,
        int? playerId
    )
    {
        IQueryable<BaseStats> baseQuery = _context.BaseStats.WhereRuleset((Ruleset)mode);

        if (chartType == LeaderboardChartType.Country && playerId.HasValue)
        {
            var playerCountry = await _context
                .Players.Where(x => x.Id == playerId)
                .Select(x => x.Country)
                .FirstOrDefaultAsync();

            // Addresses players in dependent territories having a *very* small country leaderboard.
            if (
                playerCountry != null
                && LeaderboardUtils.DependentTerritoriesMapping.TryGetValue(
                    playerCountry,
                    out var mappedCountry
                )
            )
            {
                playerCountry = mappedCountry;
            }

            baseQuery = baseQuery.Where(x => x.Player.Country == playerCountry);
        }

        if (filter == null)
        {
            return baseQuery;
        }

        baseQuery = FilterByRank(mode, baseQuery, filter.MinRank, filter.MaxRank);
        baseQuery = FilterByRating(baseQuery, filter.MinRating, filter.MaxRating);
        baseQuery = FilterByMatchesPlayed(baseQuery, filter.MinMatches, filter.MaxMatches);

        if (filter.TierFilters.IsEngaged())
        {
            baseQuery = FilterByTier(baseQuery, filter.TierFilters!);
        }

        return baseQuery;
    }

    private static IQueryable<BaseStats> FilterByRank(
        int mode,
        IQueryable<BaseStats> query,
        int? minRank,
        int? maxRank
    )
    {
        if (minRank.HasValue)
        {
            query = query.Where(x =>
                mode == 0
                    ? x.Player.RankStandard >= minRank.Value
                    : mode == 1
                        ? x.Player.RankTaiko >= minRank.Value
                        : mode == 2
                            ? x.Player.RankCatch >= minRank.Value
                            : x.Player.RankMania >= minRank.Value
            );
        }

        if (maxRank.HasValue)
        {
            query = query.Where(x =>
                mode == 0
                    ? x.Player.RankStandard <= maxRank.Value
                    : mode == 1
                        ? x.Player.RankTaiko <= maxRank.Value
                        : mode == 2
                            ? x.Player.RankCatch <= maxRank.Value
                            : x.Player.RankMania <= maxRank.Value
            );
        }

        return query;
    }

    private static IQueryable<BaseStats> FilterByRating(IQueryable<BaseStats> query, int? minRating, int? maxRating)
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

    private static IQueryable<BaseStats> FilterByMatchesPlayed(
        IQueryable<BaseStats> query,
        int? minMatches,
        int? maxMatches
    )
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

    private static IQueryable<BaseStats> FilterByTier(
        IQueryable<BaseStats> query,
        LeaderboardTierFilterDTO tierFilter
    )
    {
        var includeBronze = tierFilter.FilterBronze;
        var includeSilver = tierFilter.FilterSilver;
        var includeGold = tierFilter.FilterGold;
        var includePlatinum = tierFilter.FilterPlatinum;
        var includeEmerald = tierFilter.FilterEmerald;
        var includeDiamond = tierFilter.FilterDiamond;
        var includeMaster = tierFilter.FilterMaster;
        var includeGrandmaster = tierFilter.FilterGrandmaster;
        var includeEliteGrandmaster = tierFilter.FilterEliteGrandmaster;

        return query.Where(x =>
            (includeBronze && x.Rating < RatingUtils.RatingSilverIII)
            || (includeSilver && x.Rating >= RatingUtils.RatingSilverIII && x.Rating < RatingUtils.RatingGoldIII)
            || (includeGold && x.Rating >= RatingUtils.RatingGoldIII && x.Rating < RatingUtils.RatingPlatinumIII)
            || (includePlatinum && x.Rating >= RatingUtils.RatingPlatinumIII && x.Rating < RatingUtils.RatingEmeraldIII)
            || (includeEmerald && x.Rating >= RatingUtils.RatingEmeraldIII && x.Rating < RatingUtils.RatingDiamondIII)
            || (includeDiamond && x.Rating >= RatingUtils.RatingDiamondIII && x.Rating < RatingUtils.RatingMasterIII)
            || (includeMaster && x.Rating >= RatingUtils.RatingMasterIII && x.Rating < RatingUtils.RatingGrandmasterIII)
            || (includeGrandmaster && x.Rating >= RatingUtils.RatingGrandmasterIII && x.Rating < RatingUtils.RatingEliteGrandmaster)
            || (includeEliteGrandmaster && x.Rating >= RatingUtils.RatingEliteGrandmaster)
        );
    }
}
