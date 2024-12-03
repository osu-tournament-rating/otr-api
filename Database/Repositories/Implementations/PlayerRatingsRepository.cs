using Database.Entities.Processor;
using Database.Enums;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class PlayerRatingsRepository(OtrContext context, IPlayersRepository playersRepository)
    : RepositoryBase<PlayerRating>(context), IPlayerRatingsRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<PlayerRating>> GetForPlayerAsync(long osuPlayerId)
    {
        var id = await playersRepository.GetIdAsync(osuPlayerId);

        if (!id.HasValue)
        {
            return [];
        }

        return await _context.PlayerRatings.Where(x => x.PlayerId == id.Value).ToListAsync();
    }

    public async Task<PlayerRating?> GetForPlayerAsync(int playerId, Ruleset ruleset) =>
        await _context.PlayerRatings.Where(x => x.PlayerId == playerId && x.Ruleset == ruleset).FirstOrDefaultAsync();

    public async Task<int> BatchInsertAsync(IEnumerable<PlayerRating> playerRatings)
    {
        var ls = new List<PlayerRating>();
        foreach (PlayerRating stat in playerRatings)
        {
            ls.Add(
                new PlayerRating
                {
                    PlayerId = stat.PlayerId,
                    Ruleset = stat.Ruleset,
                    Rating = stat.Rating,
                    Volatility = stat.Volatility,
                    Percentile = stat.Percentile,
                    GlobalRank = stat.GlobalRank,
                    CountryRank = stat.CountryRank,
                    Created = DateTime.UtcNow
                }
            );
        }

        await _context.PlayerRatings.AddRangeAsync(ls);
        return await _context.SaveChangesAsync();
    }

    public async Task TruncateAsync() =>
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE base_stats RESTART IDENTITY;");

    public async Task<int> GetGlobalRankAsync(long osuPlayerId, Ruleset ruleset)
    {
        var globalIndex = (
                await _context
                    .PlayerRatings.WhereRuleset(ruleset)
                    .OrderByRatingDescending()
                    .Select(x => x.Player.OsuId)
                    .ToListAsync()
            )
            .TakeWhile(x => x != osuPlayerId)
            .Count();

        return globalIndex + 1;
    }

    public async Task<int> HighestRankAsync(Ruleset ruleset, string? country = null)
    {
        if (country != null)
        {
            return await _context
                .PlayerRatings.AsNoTracking()
                .Where(x => x.Player.Country == country && x.Ruleset == ruleset)
                .Select(x => x.CountryRank)
                .DefaultIfEmpty()
                .MaxAsync();
        }

        return await _context
            .PlayerRatings.AsNoTracking()
            .WhereRuleset(ruleset)
            .Select(x => x.GlobalRank)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public async Task<double> HighestRatingAsync(Ruleset ruleset, string? country = null)
    {
        if (country != null)
        {
            return await _context
                .PlayerRatings.AsNoTracking()
                .Where(x => x.Player.Country == country && x.Ruleset == ruleset)
                .Select(x => x.Rating)
                .DefaultIfEmpty()
                .MaxAsync();
        }

        return await _context
            .PlayerRatings.AsNoTracking()
            .Where(x => x.Ruleset == ruleset)
            .Select(x => x.Rating)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public async Task<int> HighestMatchesAsync(Ruleset ruleset, string? country = null)
    {
        if (country != null)
        {
            return await _context
                .Players.SelectMany(p => p.MatchStats)
                .Where(ms => ms.Match.Tournament.Ruleset == ruleset && ms.Player.Country == country)
                .GroupBy(ms => ms.PlayerId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Count())
                .FirstOrDefaultAsync();
        }

        return await _context
            .Players.SelectMany(p => p.MatchStats)
            .Where(ms => ms.Match.Tournament.Ruleset == ruleset)
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
}
