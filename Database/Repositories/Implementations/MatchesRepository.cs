using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Common.Enums.Queries;
using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

/// <summary>
/// Repository for managing <see cref="Match"/> entities
/// </summary>
[SuppressMessage("Performance",
    "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class MatchesRepository(OtrContext context) : Repository<Match>(context), IMatchesRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<Match>> GetAsync(
        int limit,
        int page,
        Ruleset? ruleset = null,
        string? name = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        VerificationStatus? verificationStatus = null,
        MatchRejectionReason? rejectionReason = null,
        int? submittedBy = null,
        int? verifiedBy = null,
        MatchQuerySortType? querySortType = null,
        bool? sortDescending = null
    )
    {
        IQueryable<Match> query = _context.Matches.AsQueryable();

        if (ruleset.HasValue)
        {
            query = query.WhereRuleset(ruleset.Value);
        }

        if (!string.IsNullOrEmpty(name))
        {
            query = query.WhereName(name);
        }

        if (dateMin.HasValue)
        {
            query = query.AfterDate(dateMin.Value);
        }

        if (dateMax.HasValue)
        {
            query = query.BeforeDate(dateMax.Value);
        }

        if (verificationStatus.HasValue)
        {
            query = query.Where(m => m.VerificationStatus == verificationStatus.Value);
        }

        if (rejectionReason.HasValue)
        {
            query = query.Where(m => m.RejectionReason == rejectionReason.Value);
        }


        if (submittedBy.HasValue)
        {
            query = query.Where(m => m.SubmittedByUserId == submittedBy.Value);
        }

        if (verifiedBy.HasValue)
        {
            query = query.Where(m => m.VerifiedByUserId == verifiedBy.Value);
        }

        if (querySortType.HasValue)
        {
            query = query.OrderBy(querySortType.Value, sortDescending ?? false);
        }

        return await query.Page(page, limit).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetAsync(IEnumerable<long> matchIds) =>
        await _context.Matches.AsNoTracking().Where(x => matchIds.Contains(x.OsuId)).ToListAsync();

    public async Task<Match?> GetFullAsync(int id)
    {
        IQueryable<Match> query = _context.Matches
            .AsSplitQuery()
            .AsNoTracking()
            .IncludeChildren()
            .IncludeTournament()
            .IncludeAdminNotes<Match, MatchAdminNote>();

        return await query.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Match>> SearchAsync(string name)
    {
        //_ is a wildcard character in psql so it needs to have an escape character added in front of it.
        name = name.Replace("_", @"\_");
        return await _context.Matches
            .Include(x => x.Tournament)
            .AsNoTracking()
            .WhereVerified()
            .Where(x => EF.Functions.ILike(x.Name, $"%{name}%", @"\"))
            .OrderByDescending(m => m.StartTime)
            .Take(30)
            .ToListAsync();
    }

    public async Task<Match?> MergeAsync(int parentId, IEnumerable<int> matchIds)
    {
        Match? parentMatch = await GetAsync(parentId);

        if (parentMatch is null)
        {
            return null;
        }

        ICollection<Match> childMatches = await _context.Matches
            .Include(m => m.Games)
            .Where(m => matchIds.Contains(m.Id))
            .ToListAsync();

        childMatches.ForEach(child => child.Games.ForEach(childGame =>
        {
            childGame.Match = parentMatch;
        }));

        // Save before deleting child matches
        await _context.SaveChangesAsync();

        // Delete child matches using tracked deletion to trigger auditing
        _context.Matches.RemoveRange(childMatches);
        await _context.SaveChangesAsync();

        return (await GetFullAsync(parentId))!;
    }

    public async Task LoadGamesWithScoresAsync(Match match)
    {
        // Load all games with their scores in a single query using Include chain
        await _context.Entry(match)
            .Collection(m => m.Games)
            .Query()
            .Include(g => g.Scores)
            .LoadAsync();
    }

    public async Task<IEnumerable<Match>> GetPlayerMatchesAsync(
        long osuId,
        Ruleset ruleset,
        DateTime before,
        DateTime after
    ) =>
        await _context.Matches
            .AsNoTracking()
            .WhereVerified()
            .WherePlayerParticipated(osuId)
            .WhereRuleset(ruleset)
            .WhereDateRange(after, before)
            .ToListAsync();
}
