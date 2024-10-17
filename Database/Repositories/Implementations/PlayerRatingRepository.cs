using Database.Entities.Processor;
using Database.Enums;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class PlayerRatingRepository(OtrContext context) : RepositoryBase<PlayerRating>(context), IPlayerRatingRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<PlayerRating>> GetForPlayerAsync(int id)
    {
        return await _context.PlayerRatings
            .AsNoTracking()
            .Include(pr => pr.Adjustments)
            .Where(pr => pr.PlayerId == id)
            .ToListAsync();
    }

    public async Task<PlayerRating?> GetForPlayerAsync(int playerId, Ruleset ruleset) =>
        await _context.PlayerRatings
            .AsNoTracking()
            .Include(pr => pr.Adjustments)
            .Where(pr => pr.PlayerId == playerId && pr.Ruleset == ruleset).FirstOrDefaultAsync();

    public async Task<double> HighestRatingAsync(Ruleset ruleset, string? country = null)
    {
        if (country != null)
        {
            return await _context
                .PlayerRatings
                .AsNoTracking()
                .Where(pr => pr.Player.Country == country && pr.Ruleset == ruleset)
                .Select(pr => pr.Rating)
                .DefaultIfEmpty()
                .MaxAsync();
        }

        return await _context
            .PlayerRatings
            .AsNoTracking()
            .Where(pr => pr.Ruleset == ruleset)
            .Select(pr => pr.Rating)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public async Task<int> HighestMatchesAsync(Ruleset ruleset, string? country = null)
    {
        if (country != null)
        {
            return await _context
                .Players
                .AsNoTracking()
                .SelectMany(p => p.MatchStats)
                .Where(pms => pms.Match.Tournament.Ruleset == ruleset && pms.Player.Country == country)
                .GroupBy(pms => pms.PlayerId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Count())
                .FirstOrDefaultAsync();
        }

        return await _context
            .Players
            .AsNoTracking()
            .SelectMany(p => p.MatchStats)
            .Where(pms => pms.Match.Tournament.Ruleset == ruleset)
            .GroupBy(pms => pms.PlayerId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Count())
            .FirstOrDefaultAsync();
    }

    public async Task<IDictionary<int, int>> GetHistogramAsync(Ruleset ruleset)
    {
        // Determine the maximum rating as a double
        var maxRating = await _context.PlayerRatings.Where(pr => pr.Ruleset == ruleset).MaxAsync(pr => pr.Rating);

        // Round up maxRating to the nearest multiple of 25
        var maxBucket = (int)(Math.Ceiling(maxRating / 25) * 25);

        // Initialize all buckets from 100 to maxBucket with 0
        var histogram = Enumerable.Range(4, maxBucket / 25 - 3).ToDictionary(bucket => bucket * 25, _ => 0);

        // Adjust the GroupBy to correctly bucket the rating of 100
        Dictionary<int, int> dbHistogram = await _context
            .PlayerRatings
            .AsNoTracking()
            .Where(pr => pr.Ruleset == ruleset && pr.Rating >= 100)
            .GroupBy(pr => (int)(pr.Rating / 25) * 25)
            .Select(g => new { Bucket = g.Key == 0 ? 100 : g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Bucket, g => g.Count);

        // Update the histogram with actual counts
        foreach (KeyValuePair<int, int> item in dbHistogram.Where(item => histogram.ContainsKey(item.Key)))
        {
            histogram[item.Key] = item.Value;
        }

        return histogram;
    }
}
