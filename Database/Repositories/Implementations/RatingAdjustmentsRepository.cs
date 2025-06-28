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

    public async Task<IEnumerable<RatingAdjustment>> GetForPlayersAsync(
        IEnumerable<int> playerIds,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;
        var playerIdsList = playerIds.ToList();

        return await _context.RatingAdjustments
            .Where(ra =>
                playerIdsList.Contains(ra.PlayerId)
                && ra.Ruleset == ruleset
                && ra.Timestamp >= dateMin
                && ra.Timestamp <= dateMax
            )
            .Include(ra => ra.Match!.Tournament)
            .ToListAsync();
    }
}
