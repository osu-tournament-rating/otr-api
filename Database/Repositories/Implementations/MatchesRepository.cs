using System.Diagnostics.CodeAnalysis;
using Common.Enums.Enums;
using Common.Enums.Enums.Queries;
using Common.Enums.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance",
    "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class MatchesRepository(OtrContext context) : RepositoryBase<Match>(context), IMatchesRepository
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
        MatchProcessingStatus? processingStatus = null,
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

        if (processingStatus.HasValue)
        {
            query = query.Where(m => m.ProcessingStatus == processingStatus.Value);
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

        return await query.Page(limit, page - 1).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetAsync(IEnumerable<long> matchIds) =>
        await _context.Matches.Where(x => matchIds.Contains(x.OsuId)).ToListAsync();

    public async Task<Match?> GetFullAsync(int id, bool verified)
    {
        IQueryable<Match> query = _context.Matches
            .AsNoTracking()
            .IncludeChildren(verified)
            .IncludeTournament()
            .IncludeAdminNotes<Match, MatchAdminNote>();

        return await query.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Match>> SearchAsync(string name)
    {
        //_ is a wildcard character in psql so it needs to have an escape character added in front of it.
        name = name.Replace("_", @"\_");
        return await _context.Matches
            .AsNoTracking()
            .WhereVerified()
            .WhereProcessingCompleted()
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
        Match? match = await GetAsync(id);
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
            .WhereVerified()
            .WhereProcessingCompleted()
            .WherePlayerParticipated(osuId)
            .WhereRuleset(ruleset)
            .WhereDateRange(after, before)
            .ToListAsync();
}
