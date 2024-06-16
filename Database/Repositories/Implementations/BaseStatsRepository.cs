using Database.Entities;
using Database.Entities.Processor;
using Database.Enums;
using Database.Extensions;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class BaseStatsRepository(OtrContext context, IPlayersRepository playersRepository) : RepositoryBase<BaseStats>(context), IBaseStatsRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<BaseStats>> GetForPlayerAsync(long osuPlayerId)
    {
        var id = await playersRepository.GetIdAsync(osuPlayerId);

        if (!id.HasValue)
        {
            return new List<BaseStats>();
        }

        return await _context.BaseStats.Where(x => x.PlayerId == id.Value).ToListAsync();
    }

    public async Task<BaseStats?> GetForPlayerAsync(int playerId, int mode) =>
        await _context.BaseStats.Where(x => x.PlayerId == playerId && x.Mode == (Ruleset)mode).FirstOrDefaultAsync();

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
                .Where(ms => ms.Match.Tournament.Ruleset == (Ruleset)mode && ms.Player.Country == country)
                .GroupBy(ms => ms.PlayerId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Count())
                .FirstOrDefaultAsync();
        }

        return await _context
            .Players.SelectMany(p => p.MatchStats)
            .Where(ms => ms.Match.Tournament.Ruleset == (Ruleset)mode)
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
}
