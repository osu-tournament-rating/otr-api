using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Queries.Extensions;
using Database.Queries.Filters;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class MatchesRepository(
    OtrContext context
) : RepositoryBase<Match>(context), IMatchesRepository
{
    private readonly OtrContext _context = context;

    public async Task<Match?> GetAsync(
        int id,
        QueryFilterType filterType = QueryFilterType.Verified | QueryFilterType.ProcessingCompleted
    ) =>
        await _context.Matches
            .WhereFiltered(filterType)
            .IncludeChildren(filterType)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<Match>> GetAsync(
        int limit,
        int page,
        MatchesQueryFilter filter,
        bool tracking = true
    )
    {
        IQueryable<Match> query = _context.Matches.WhereFiltered(filter);
        query = tracking ? query : query.AsNoTracking();

        return await query.Page(limit, page).ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetAsync(
        int limit,
        int page,
        QueryFilterType filterType = QueryFilterType.Verified | QueryFilterType.ProcessingCompleted
    ) =>
        await _context.Matches
            .WhereFiltered(filterType)
            .IncludeChildren(filterType)
            .Include(m => m.Tournament)
            .OrderBy(m => m.Id)
            .Page(limit, page)
            .ToListAsync();

    public async Task<IEnumerable<Match>> GetAsync(IEnumerable<long> matchIds) =>
        await _context.Matches.Where(x => matchIds.Contains(x.OsuId)).ToListAsync();

    public async Task<Match?> GetByOsuIdAsync(long osuId) =>
        await _context
            .Matches
            .IncludeChildren(QueryFilterType.None)
            .FirstOrDefaultAsync(x => x.OsuId == osuId);

    public async Task<IEnumerable<Match>> SearchAsync(string name)
    {
        //_ is a wildcard character in psql so it needs to have an escape character added in front of it.
        name = name.Replace("_", @"\_");
        return await _context.Matches
            .AsNoTracking()
            .WhereFiltered(QueryFilterType.Verified | QueryFilterType.ProcessingCompleted)
            .Where(x => EF.Functions.ILike(x.Name, $"%{name}%", @"\"))
            .Take(30)
            .ToListAsync();
    }

    public async Task<Match?> UpdateVerificationStatusAsync(
        int id,
        VerificationStatus status,
        int? verifierId = null
    )
    {
        Match? match = await GetAsync(id, QueryFilterType.None);
        if (match is null)
        {
            return null;
        }

        match.VerificationStatus = status;
        await UpdateAsync(match);

        return match;
    }

    public async Task<IEnumerable<Match>> GetPlayerMatchesAsync(
        long osuId,
        Ruleset ruleset,
        DateTime before,
        DateTime after
    ) =>
        await _context.Matches
            .WhereFiltered(QueryFilterType.Verified | QueryFilterType.ProcessingCompleted)
            .IncludeChildren(QueryFilterType.Verified | QueryFilterType.ProcessingCompleted)
            .WherePlayerParticipated(osuId)
            .WhereRuleset(ruleset)
            .WhereDateRange(after, before)
            .ToListAsync();
}
