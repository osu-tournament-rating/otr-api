using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Entities.Processor;
using Database.Enums;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class MatchRatingStatsRepository(OtrContext context) : RepositoryBase<MatchRatingStats>(context), IMatchRatingStatsRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<IEnumerable<MatchRatingStats>>> GetForPlayerAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return await _context
            .MatchRatingStats.Where(x =>
                x.PlayerId == playerId
                && x.Match.Tournament.Ruleset == (Ruleset)mode
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
            )
            .Include(x => x.Match)
            .ThenInclude(x => x.Tournament)
            .GroupBy(x => x.Match.StartTime.Date)
            .ToListAsync();
    }

    public async Task TruncateAsync() =>
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE match_rating_stats RESTART IDENTITY");

    public async Task<int> HighestGlobalRankAsync(
        int playerId,
        int mode,
        DateTime? dateMin,
        DateTime? dateMax
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return await _context
            .MatchRatingStats.Where(x =>
                x.PlayerId == playerId
                && x.Match.Tournament.Ruleset == (Ruleset)mode
                && x.Match.StartTime != null
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
            )
            .Select(x => x.GlobalRankAfter)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public async Task<int> HighestCountryRankAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return await _context
            .MatchRatingStats.Where(x =>
                x.PlayerId == playerId
                && x.Match.Tournament.Ruleset == (Ruleset)mode
                && x.Match.StartTime != null
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
            )
            .Select(x => x.CountryRankAfter)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public async Task<DateTime?> GetOldestForPlayerAsync(int playerId, int mode) =>
        await _context
            .MatchRatingStats.Where(x =>
                x.PlayerId == playerId && x.Match.Tournament.Ruleset == (Ruleset)mode && x.Match.StartTime != null
            )
            .Select(x => x.Match.StartTime)
            .MinAsync();

    public async Task<IEnumerable<MatchRatingStats>> TeammateRatingStatsAsync(
        int playerId,
        int teammateId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    ) =>
        await _context
            .MatchRatingStats.Where(mrs => mrs.PlayerId == playerId)
            .Where(mrs =>
                _context.PlayerMatchStats.Any(pms =>
                    pms.PlayerId == mrs.PlayerId
                    && pms.TeammateIds.Contains(teammateId)
                    && pms.Match.Tournament.Ruleset == (Ruleset)mode
                    && pms.Match.StartTime >= dateMin
                    && pms.Match.StartTime <= dateMax
                )
            )
            .Distinct()
            .ToListAsync();

    public async Task<IEnumerable<MatchRatingStats>> OpponentRatingStatsAsync(
        int playerId,
        int opponentId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    ) =>
        await _context
            .MatchRatingStats.Where(mrs => mrs.PlayerId == playerId)
            .Where(mrs =>
                _context.PlayerMatchStats.Any(pms =>
                    pms.PlayerId == mrs.PlayerId
                    && pms.OpponentIds.Contains(opponentId)
                    && pms.Match.Tournament.Ruleset == (Ruleset)mode
                    && pms.Match.StartTime >= dateMin
                    && pms.Match.StartTime <= dateMax
                )
            )
            .Distinct()
            .ToListAsync();
}
