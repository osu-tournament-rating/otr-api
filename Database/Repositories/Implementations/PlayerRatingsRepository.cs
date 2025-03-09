using Common.Enums;
using Common.Enums.Enums;
using Common.Rating;
using Common.Utilities;
using Database.Entities;
using Database.Entities.Processor;
using Database.Models;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class PlayerRatingsRepository(OtrContext context)
    : RepositoryBase<PlayerRating>(context), IPlayerRatingsRepository
{
    private readonly OtrContext _context = context;

    public async Task<PlayerRating?> GetAsync(int playerId, Ruleset ruleset, bool includeAdjustments)
    {
        if (includeAdjustments)
        {
            return await _context.PlayerRatings
                .Include(pr => pr.Player.User)
                .Include(pr => pr.Adjustments)
                .ThenInclude(a => a.Match)
                .Where(pr => pr.PlayerId == playerId && pr.Ruleset == ruleset)
                .FirstOrDefaultAsync();
        }

        return await _context.PlayerRatings
            .Include(pr => pr.Player.User)
            .Where(pr => pr.PlayerId == playerId && pr.Ruleset == ruleset)
            .FirstOrDefaultAsync();
    }

    public async Task<IList<Ruleset>> GetActiveRulesetsAsync(int playerId) =>
        await _context.PlayerRatings
            .Where(pr => pr.PlayerId == playerId)
            .Select(pr => pr.Ruleset)
            .ToListAsync();


    public async Task<int> HighestRankAsync(Ruleset ruleset, string? country = null)
    {
        IQueryable<PlayerRating> query = _context
            .PlayerRatings
            .AsNoTracking()
            .WhereRuleset(ruleset);

        if (country != null)
        {
            query = query.Where(pr => pr.Player.Country == country);
        }

        return await query
            .Select(x => x.GlobalRank)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public async Task<double> HighestRatingAsync(Ruleset ruleset, string? country = null)
    {
        IQueryable<PlayerRating> query = _context
            .PlayerRatings
            .AsNoTracking()
            .Where(x => x.Ruleset == ruleset);

        if (country != null)
        {
            query = query.Where(pr => pr.Player.Country == country);
        }

        return await query
            .Select(x => x.Rating)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public async Task<int> HighestMatchesAsync(Ruleset ruleset, string? country = null)
    {
        IQueryable<PlayerMatchStats> query = _context
            .Players
            .AsNoTracking()
            .SelectMany(p => p.MatchStats)
            .Where(ms => ms.Match.Tournament.Ruleset == ruleset);

        if (country != null)
        {
            query = query.Where(pms => pms.Player.Country == country);
        }

        return await query
            .GroupBy(ms => ms.PlayerId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Count())
            .FirstOrDefaultAsync();
    }

    public async Task<IDictionary<int, int>> GetHistogramAsync(Ruleset ruleset)
    {
        // Determine the maximum rating as a double
        var maxRating = await _context.PlayerRatings.Where(x => x.Ruleset == ruleset).MaxAsync(x => x.Rating);

        // Round up maxRating to the nearest multiple of 25
        var maxBucket = (int)(Math.Ceiling(maxRating / 25) * 25);

        // Initialize all buckets from 100 to maxBucket with 0
        var histogram = Enumerable.Range(4, maxBucket / 25 - 3).ToDictionary(bucket => bucket * 25, _ => 0);

        // Adjust the GroupBy to correctly bucket the rating of 100
        Dictionary<int, int> dbHistogram = await _context
            .PlayerRatings.AsNoTracking()
            .Where(x => x.Ruleset == ruleset && x.Rating >= 100)
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

    public async Task<IEnumerable<PlayerRating>> GetLeaderboardAsync(
        int page,
        int pageSize,
        Ruleset ruleset,
        LeaderboardChartType chartType,
        LeaderboardFilter? filter,
        string? country
    )
    {
        IQueryable<PlayerRating> query = LeaderboardQuery(
            ruleset,
            chartType,
            filter,
            country
        );

        return await query
            .OrderByRatingDescending()
            .Page(pageSize, page - 1)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> LeaderboardCountAsync(
        Ruleset requestQueryRuleset,
        LeaderboardChartType chartType,
        LeaderboardFilter filter,
        string? country
    )
    {
        IQueryable<PlayerRating> query = LeaderboardQuery(requestQueryRuleset, chartType, filter, country);

        return await query.CountAsync();
    }

    private IQueryable<PlayerRating> LeaderboardQuery(
        Ruleset ruleset,
        LeaderboardChartType chartType,
        LeaderboardFilter? filter,
        string? country
    )
    {
        IQueryable<PlayerRating> baseQuery = _context.PlayerRatings
            .WhereRuleset(ruleset)
            // Filter out players who only have the initial adjustment
            .Where(pr => pr.Adjustments.Count > 1);

        if (chartType == LeaderboardChartType.Country && country is not null)
        {
            // Addresses players in dependent territories having a *very* small country leaderboard.
            if (
                LeaderboardUtils.DependentTerritoriesMapping.TryGetValue(
                    country,
                    out var mappedCountry
                )
            )
            {
                baseQuery = baseQuery.Where(x => x.Player.Country == mappedCountry);
            }
        }

        if (filter == null)
        {
            return baseQuery;
        }

        baseQuery = FilterByRank(ruleset, baseQuery, filter.MinRank, filter.MaxRank);
        baseQuery = FilterByRating(baseQuery, filter.MinRating, filter.MaxRating);
        baseQuery = FilterByMatchesPlayed(baseQuery, filter.MinMatches, filter.MaxMatches);

        if (filter?.TierFilters != null && HasActiveTierFilter(filter.TierFilters))
        {
            baseQuery = FilterByTier(baseQuery, filter.TierFilters);
        }

        return baseQuery;
    }

    private static bool HasActiveTierFilter(LeaderboardTierFilter tierFilter)
    {
        return tierFilter.FilterBronze || tierFilter.FilterSilver ||
               tierFilter.FilterGold || tierFilter.FilterPlatinum ||
               tierFilter.FilterEmerald || tierFilter.FilterDiamond ||
               tierFilter.FilterMaster || tierFilter.FilterGrandmaster ||
               tierFilter.FilterEliteGrandmaster;
    }

    private static IQueryable<PlayerRating> FilterByRank(
        Ruleset ruleset,
        IQueryable<PlayerRating> query,
        int? minRank,
        int? maxRank
    )
    {
        if (minRank.HasValue)
        {
            query = query.Where(x =>
                x.Player.RulesetData.Any(rd => rd.Ruleset == ruleset)
                && x.Player.RulesetData.FirstOrDefault(rd => rd.Ruleset == ruleset)!.GlobalRank >= minRank
            );
        }

        if (maxRank.HasValue)
        {
            query = query.Where(x =>
                x.Player.RulesetData.Any(rd => rd.Ruleset == ruleset)
                && x.Player.RulesetData.FirstOrDefault(rd => rd.Ruleset == ruleset)!.GlobalRank <= maxRank
            );
        }

        return query;
    }

    private static IQueryable<PlayerRating> FilterByRating(IQueryable<PlayerRating> query, int? minRating,
        int? maxRating)
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

    private static IQueryable<PlayerRating> FilterByMatchesPlayed(
        IQueryable<PlayerRating> query,
        int? minMatches,
        int? maxMatches
    )
    {
        if (minMatches.HasValue || maxMatches.HasValue)
        {
            query = query.Include(x => x.Player).ThenInclude(x => x.MatchStats);
        }

        if (minMatches.HasValue)
        {
            query = query.Where(x => x.Player.MatchStats.Count >= minMatches.Value);
        }

        if (maxMatches.HasValue)
        {
            query = query.Where(x => x.Player.MatchStats.Count <= maxMatches.Value);
        }

        return query;
    }

    private static IQueryable<PlayerRating> FilterByTier(
        IQueryable<PlayerRating> query,
        LeaderboardTierFilter tierFilter
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
            (includeBronze && x.Rating < RatingConstants.RatingSilverIII)
            || (includeSilver && x.Rating >= RatingConstants.RatingSilverIII && x.Rating < RatingConstants.RatingGoldIII)
            || (includeGold && x.Rating >= RatingConstants.RatingGoldIII && x.Rating < RatingConstants.RatingPlatinumIII)
            || (includePlatinum && x.Rating >= RatingConstants.RatingPlatinumIII && x.Rating < RatingConstants.RatingEmeraldIII)
            || (includeEmerald && x.Rating >= RatingConstants.RatingEmeraldIII && x.Rating < RatingConstants.RatingDiamondIII)
            || (includeDiamond && x.Rating >= RatingConstants.RatingDiamondIII && x.Rating < RatingConstants.RatingMasterIII)
            || (includeMaster && x.Rating >= RatingConstants.RatingMasterIII && x.Rating < RatingConstants.RatingGrandmasterIII)
            || (includeGrandmaster && x.Rating >= RatingConstants.RatingGrandmasterIII &&
                x.Rating < RatingConstants.RatingEliteGrandmaster)
            || (includeEliteGrandmaster && x.Rating >= RatingConstants.RatingEliteGrandmaster)
        );
    }
}
