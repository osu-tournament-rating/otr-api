using Common.Enums;
using Database.Entities.Processor;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class RatingAdjustmentsRepository(OtrContext context)
    : Repository<RatingAdjustment>(context), IRatingAdjustmentsRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<RatingAdjustment>> GetForPlayerAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return await _context.RatingAdjustments
            .Where(ra =>
                ra.PlayerId == playerId
                && ra.Ruleset == ruleset
                && ra.Timestamp >= dateMin
                && ra.Timestamp <= dateMax
            )
            .Include(ra => ra.Match!.Tournament)
            .ToListAsync();
    }

    public async Task<Dictionary<int, double?>> GetPeakRatingsForPlayersAsync(
        IEnumerable<int> playerIds,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        var playerIdsList = playerIds.ToList();

        if (playerIdsList.Count == 0)
        {
            return new Dictionary<int, double?>();
        }

        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        // Execute optimized query that calculates max rating per player in the database
        Dictionary<int, double?> peakRatings = await _context.RatingAdjustments
            .AsNoTracking()
            .Where(ra =>
                playerIdsList.Contains(ra.PlayerId) &&
                ra.Ruleset == ruleset &&
                ra.Timestamp >= dateMin &&
                ra.Timestamp <= dateMax)
            .GroupBy(ra => ra.PlayerId)
            .Select(g => new { PlayerId = g.Key, PeakRating = g.Max(ra => ra.RatingAfter) })
            .ToDictionaryAsync(x => x.PlayerId, x => (double?)x.PeakRating);

        return peakRatings;
    }
}
