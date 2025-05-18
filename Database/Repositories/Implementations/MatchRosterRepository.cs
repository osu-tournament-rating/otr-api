using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class MatchRosterRepository(OtrContext context) : RepositoryBase<MatchRoster>(context), IMatchRosterRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<MatchRoster>> FetchRostersAsync(int playerId, Ruleset ruleset, DateTime? dateMin, DateTime? dateMax = null,
        int limit = 5)
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return await _context.MatchRosters
            .Where(mr => mr.Roster.Contains(playerId) && mr.Match.Tournament.Ruleset == ruleset &&
                         mr.Match.StartTime >= dateMin && mr.Match.StartTime <= dateMax)
            .ToListAsync();
    }
}
