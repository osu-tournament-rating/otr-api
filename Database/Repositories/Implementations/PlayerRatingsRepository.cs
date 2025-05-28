using Common.Constants;
using Common.Enums;
using Common.Utilities;
using Database.Entities.Processor;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class PlayerRatingsRepository(OtrContext context)
    : RepositoryBase<PlayerRating>(context), IPlayerRatingsRepository
{
    private readonly OtrContext _context = context;

    public async Task<PlayerRating?> GetAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null, bool includeAdjustments = false)
    {
        if (includeAdjustments)
        {
            return await _context.PlayerRatings
                .Include(pr => pr.Player.User)
                .Include(pr => pr.Adjustments.Where(ra => ra.Timestamp >= dateMin && ra.Timestamp <= dateMax))
                .ThenInclude(a => a.Match)
                .Where(pr => pr.PlayerId == playerId && pr.Ruleset == ruleset)
                .FirstOrDefaultAsync();
        }

        return await _context.PlayerRatings
            .Include(pr => pr.Player.User)
            .Where(pr => pr.PlayerId == playerId && pr.Ruleset == ruleset)
            .FirstOrDefaultAsync();
    }

    public async Task<IList<PlayerRating>> GetAsync(int page = 1, int pageSize = 25, Ruleset ruleset = Ruleset.Osu,
        string? country = null,
        int? minRank = null, int? maxRank = null, int? minRating = null, int? maxRating = null, int? minMatches = null,
        int? maxMatches = null, double? minWinRate = null, double? maxWinRate = null, bool bronze = false,
        bool silver = false, bool gold = false, bool platinum = false, bool emerald = false, bool diamond = false,
        bool master = false, bool grandmaster = false, bool eliteGrandmaster = false)
    {
        return await LeaderboardQuery(ruleset, country, minRank, maxRank, minRating, maxRating,
                minMatches, maxMatches, minWinRate, maxWinRate, bronze, silver, gold, platinum, emerald, diamond,
                master,
                grandmaster, eliteGrandmaster)
            .OrderByDescending(pr => pr.Rating)
            .Page(page, pageSize)
            .ToListAsync();
    }

    public async Task<int> PageCountAsync(int pageSize = 25, Ruleset ruleset = Ruleset.Osu,
        string? country = null, int? minRank = null,
        int? maxRank = null, int? minRating = null, int? maxRating = null, int? minMatches = null,
        int? maxMatches = null,
        double? minWinRate = null, double? maxWinRate = null, bool bronze = false, bool silver = false,
        bool gold = false,
        bool platinum = false, bool emerald = false, bool diamond = false, bool master = false,
        bool grandmaster = false,
        bool eliteGrandmaster = false) =>
        await LeaderboardQuery(ruleset, country, minRank, maxRank, minRating, maxRating, minMatches, maxMatches,
            minWinRate, maxWinRate, bronze, silver, gold, platinum, emerald, diamond, master, grandmaster,
            eliteGrandmaster).CountAsync() / pageSize + 1;

    public async Task<IList<Ruleset>> GetActiveRulesetsAsync(int playerId) =>
        await _context.PlayerRatings
            .Include(pr => pr.Player)
            .Where(pr => pr.PlayerId == playerId)
            .Select(pr => pr.Ruleset)
            .ToListAsync();

    public async Task<Dictionary<Ruleset, Dictionary<int, int>>> GetHistogramAsync()
    {
        const int bucketSize = 25;
        const int minRating = 100;

        var histograms = await _context.PlayerRatings
            .Where(x => x.Rating >= minRating)
            .GroupBy(x => new
            {
                x.Ruleset,
                Bucket = Math.Floor(x.Rating / bucketSize) * bucketSize
            })
            .Select(g => new
            {
                g.Key.Ruleset,
                g.Key.Bucket,
                Count = g.Count()
            })
            .ToListAsync();

        var result = new Dictionary<Ruleset, Dictionary<int, int>>();
        Ruleset[] rulesets = Enum.GetValues<Ruleset>();

        foreach (Ruleset ruleset in rulesets)
        {
            var rulesetHistogram = new Dictionary<int, int>();
            var rulesetBuckets = histograms.Where(h => h.Ruleset == ruleset).ToArray();

            if (rulesetBuckets.Length != 0)
            {
                var maxBucket = rulesetBuckets.Max(h => h.Bucket);

                for (var bucket = minRating; bucket <= maxBucket; bucket += bucketSize)
                {
                    rulesetHistogram[bucket] = rulesetBuckets.FirstOrDefault(h => h.Bucket == bucket)?.Count ?? 0;
                }
            }

            result[ruleset] = rulesetHistogram;
        }

        return result;
    }

    private IQueryable<PlayerRating> LeaderboardQuery(
        Ruleset ruleset = Ruleset.Osu, string? country = null,
        int? minRank = null, int? maxRank = null, int? minRating = null, int? maxRating = null, int? minMatches = null,
        int? maxMatches = null, double? minWinRate = null, double? maxWinRate = null, bool bronze = false,
        bool silver = false, bool gold = false, bool platinum = false, bool emerald = false, bool diamond = false,
        bool master = false, bool grandmaster = false, bool eliteGrandmaster = false
    )
    {
        IQueryable<PlayerRating> baseQuery = _context.PlayerRatings
            .AsNoTracking()
            .Include(pr => pr.Player)
            .AsSplitQuery()
            .WhereRuleset(ruleset)
            // Filter out players who only have the initial adjustment
            .Where(pr => pr.Adjustments.Count > 1);

        baseQuery = FilterByCountry(country, baseQuery);
        baseQuery = FilterByRank(ruleset, baseQuery, minRank, maxRank);
        baseQuery = FilterByRating(baseQuery, minRating, maxRating);
        baseQuery = FilterByMatchesPlayed(baseQuery, ruleset, minMatches, maxMatches);
        baseQuery = FilterByWinRate(baseQuery, ruleset, minWinRate, maxWinRate);
        baseQuery = FilterByTier(baseQuery, bronze, silver, gold, platinum, emerald, diamond, master, grandmaster,
            eliteGrandmaster);

        return baseQuery;
    }

    private static IQueryable<PlayerRating> FilterByCountry(string? country, IQueryable<PlayerRating> baseQuery)
    {
        if (country is null)
        {
            return baseQuery;
        }

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

        return baseQuery;
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
                x.Player.RulesetData.FirstOrDefault(rd => rd.Ruleset == ruleset)!.GlobalRank >= minRank
            );
        }

        if (maxRank.HasValue)
        {
            query = query.Where(x =>
                x.Player.RulesetData.FirstOrDefault(rd => rd.Ruleset == ruleset)!.GlobalRank <= maxRank
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
        Ruleset ruleset,
        int? minMatches,
        int? maxMatches
    )
    {
        if (minMatches.HasValue)
        {
            query = query.Where(x =>
                x.Player.TournamentStats.Where(pts => pts.Tournament.Ruleset == ruleset).Sum(pts => pts.MatchesPlayed) >= minMatches.Value);
        }

        if (maxMatches.HasValue)
        {
            query = query.Where(x =>
                x.Player.TournamentStats.Where(pts => pts.Tournament.Ruleset == ruleset).Sum(pts => pts.MatchesPlayed) <= maxMatches.Value);
        }

        return query;
    }

    private static IQueryable<PlayerRating> FilterByWinRate(
        IQueryable<PlayerRating> query,
        Ruleset ruleset,
        double? minWinRate,
        double? maxWinRate
    )
    {
        if (minWinRate.HasValue)
        {
            query = query.Where(pr =>
                pr.Player.TournamentStats
                    .Where(ts => ts.Tournament.Ruleset == ruleset)
                    .Average(ts => ts.MatchesWon / (double)ts.MatchesPlayed) >= minWinRate.Value);
        }

        if (maxWinRate.HasValue)
        {
            query = query.Where(pr =>
                pr.Player.TournamentStats
                    .Where(ts => ts.Tournament.Ruleset == ruleset)
                    .Average(ts => ts.MatchesWon / (double)ts.MatchesPlayed) <= maxWinRate.Value);
        }

        return query;
    }

    private static IQueryable<PlayerRating> FilterByTier(
        IQueryable<PlayerRating> query,
        bool bronze, bool silver, bool gold, bool platinum, bool emerald, bool diamond,
        bool master, bool grandmaster, bool eliteGrandmaster
    )
    {
        // Check if all flags are true or all are false
        var allTrue = bronze && silver && gold && platinum && emerald && diamond && master && grandmaster &&
                      eliteGrandmaster;
        var allFalse = !bronze && !silver && !gold && !platinum && !emerald && !diamond && !master && !grandmaster &&
                       !eliteGrandmaster;

        // If all flags are true or all are false, return the original query
        if (allTrue || allFalse)
        {
            return query;
        }

        // Otherwise, build the predicate dynamically
        ExpressionStarter<PlayerRating>? predicate = PredicateBuilder.New<PlayerRating>(false);

        if (bronze)
        {
            predicate = predicate.Or(p =>
                p.Rating >= RatingConstants.RatingBronzeIII && p.Rating < RatingConstants.RatingSilverIII);
        }

        if (silver)
        {
            predicate = predicate.Or(p =>
                p.Rating >= RatingConstants.RatingSilverIII && p.Rating < RatingConstants.RatingGoldIII);
        }

        if (gold)
        {
            predicate = predicate.Or(p =>
                p.Rating >= RatingConstants.RatingGoldIII && p.Rating < RatingConstants.RatingPlatinumIII);
        }

        if (platinum)
        {
            predicate = predicate.Or(p =>
                p.Rating >= RatingConstants.RatingPlatinumIII && p.Rating < RatingConstants.RatingEmeraldIII);
        }

        if (emerald)
        {
            predicate = predicate.Or(p =>
                p.Rating >= RatingConstants.RatingEmeraldIII && p.Rating < RatingConstants.RatingDiamondIII);
        }

        if (diamond)
        {
            predicate = predicate.Or(p =>
                p.Rating >= RatingConstants.RatingDiamondIII && p.Rating < RatingConstants.RatingMasterIII);
        }

        if (master)
        {
            predicate = predicate.Or(p =>
                p.Rating >= RatingConstants.RatingMasterIII && p.Rating < RatingConstants.RatingGrandmasterIII);
        }

        if (grandmaster)
        {
            predicate = predicate.Or(p =>
                p.Rating >= RatingConstants.RatingGrandmasterIII && p.Rating < RatingConstants.RatingEliteGrandmaster);
        }

        if (eliteGrandmaster)
        {
            predicate = predicate.Or(p => p.Rating >= RatingConstants.RatingEliteGrandmaster);
        }

        // Apply the predicate to the query
        return query.AsExpandable().Where(predicate);
    }
}
