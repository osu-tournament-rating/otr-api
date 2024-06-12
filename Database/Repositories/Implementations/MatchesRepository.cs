using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Enums;
using Database.Extensions;
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

    public override async Task<Match?> GetAsync(int id) =>
        // Get the match with all associated data
        await MatchBaseQuery(false).FirstOrDefaultAsync(x => x.Id == id);

    // Suppression: This query will inherently produce a large number of records by including
    // games and match scores. The query itself is almost as efficient as possible (as far as we know)
    [SuppressMessage("ReSharper.DPA", "DPA0007: Large number of DB records")]
    public async Task<IEnumerable<Match>> GetAsync(int limit, int page, bool filterUnverified = true) =>
        await MatchBaseQuery(filterUnverified)
            // Include all MatchDTO navigational properties
            .Include(m => m.Tournament)
            .OrderBy(m => m.Id)
            // Set index to start of desired page
            .Skip(limit * page)
            // Take only next n entities
            .Take(limit)
            .ToListAsync();

    public async Task<IEnumerable<Match>> SearchAsync(string name)
    {
        //_ is a wildcard character in psql so it needs to have an escape character added in front of it.
        name = name.Replace("_", @"\_");
        return await _context.Matches
            .AsNoTracking()
            .WhereVerified()
            .Where(x => EF.Functions.ILike(x.Name ?? string.Empty, $"%{name}%", @"\"))
            .Take(30)
            .ToListAsync();
    }

    public async Task<Match?> GetAsync(int id, bool filterInvalidMatches = true) =>
        await MatchBaseQuery(filterInvalidMatches).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Match?> GetByMatchIdAsync(long matchId) =>
        await _context
            .Matches.Include(x => x.Games)
            .ThenInclude(x => x.MatchScores)
            .Include(x => x.Tournament)
            .FirstOrDefaultAsync(x => x.MatchId == matchId);

    public async Task<IList<Match>> GetMatchesNeedingAutoCheckAsync(int limit = 10000) =>
        // We only want api processed matches because the verification checks require the data from the API
        await _context
            .Matches.Include(x => x.Games)
            .ThenInclude(x => x.MatchScores)
            .Include(x => x.Tournament)
            .Where(x => x.NeedsAutoCheck == true && x.IsApiProcessed == true)
            .Take(limit)
            .ToListAsync();

    public async Task<Match?> GetFirstMatchNeedingApiProcessingAsync() =>
        await _context
            .Matches.Include(x => x.Games)
            .ThenInclude(x => x.MatchScores)
            .Where(x => x.IsApiProcessed == false)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Match>> GetAsync(IEnumerable<long> matchIds) =>
        await _context.Matches.Where(x => matchIds.Contains(x.MatchId)).ToListAsync();

    public async Task<Match?> UpdateVerificationStatusAsync(
        int id,
        Old_MatchVerificationStatus status,
        Old_MatchVerificationSource source,
        string? info = null,
        int? verifierId = null
    )
    {
        Match? match = await GetAsync(id);
        if (match is null)
        {
            return null;
        }

        match.VerificationStatus = status;
        match.VerificationSource = source;
        match.VerificationInfo = info;
        await UpdateAsync(match);

        return match;
    }

    public async Task<IEnumerable<Match>> GetPlayerMatchesAsync(
        long osuId,
        int mode,
        DateTime before,
        DateTime after
    )
    {
        return await QueryExtensions.WhereRuleset(_context
                .Matches.IncludeAllChildren()
                .WherePlayerParticipated(osuId), (Ruleset)mode)
            .Before(before)
            .After(after)
            .ToListAsync();
    }

    public async Task UpdateAsApiProcessed(Match match)
    {
        match.IsApiProcessed = true;
        await UpdateAsync(match);
    }

    public async Task SetRequireAutoCheckAsync(bool invalidOnly = true)
    {
        if (invalidOnly)
        {
            await _context
                .Matches.Where(x =>
                    x.VerificationStatus != Old_MatchVerificationStatus.Verified
                    && x.VerificationStatus != Old_MatchVerificationStatus.PreVerified
                )
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.NeedsAutoCheck, true));
        }
        else
        {
            // Applies to all matches
            await _context.Matches.ExecuteUpdateAsync(x => x.SetProperty(y => y.NeedsAutoCheck, true));
        }
    }

    private IQueryable<Match> MatchBaseQuery(bool filterInvalidMatches)
    {
        if (!filterInvalidMatches)
        {
            return _context
                .Matches.Include(x => x.Games)
                .ThenInclude(x => x.MatchScores)
                .Include(x => x.Games)
                .ThenInclude(x => x.Beatmap);
        }

        return _context
            .Matches.WhereVerified()
            .Include(x => x.Games.Where(y => y.VerificationStatus == Old_GameVerificationStatus.Verified))
            .ThenInclude(x => x.MatchScores.Where(y => y.IsValid == true))
            .Include(x => x.Games.Where(y => y.VerificationStatus == Old_GameVerificationStatus.Verified))
            .ThenInclude(x => x.Beatmap);
    }
}
