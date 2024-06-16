using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Entities.Processor;
using Database.Enums;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class TournamentsRepository(OtrContext context) : RepositoryBase<Tournament>(context), ITournamentsRepository
{
    private readonly OtrContext _context = context;

    public async Task<Tournament?> GetAsync(int id, bool eagerLoad = false) =>
        eagerLoad ? await TournamentsBaseQuery().FirstOrDefaultAsync(x => x.Id == id) : await base.GetAsync(id);

    public async Task<bool> ExistsAsync(string name, int mode) =>
        await _context.Tournaments.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Ruleset == (Ruleset)mode);

    public async Task<int> CountPlayedAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    ) => await QueryForParticipation(playerId, mode, dateMin, dateMax).Select(x => x.Id).Distinct().CountAsync();

    /// <summary>
    /// Returns a queryable containing tournaments for <see cref="mode"/>
    /// with *any* match applicable to all of the following criteria:
    /// - Is verified
    /// - Started between <paramref name="dateMin"/> and <paramref name="dateMax"/>
    /// - Contains a <see cref="MatchRatingStats"/> for given <paramref name="playerId"/> (Denotes participation)
    /// </summary>
    /// <param name="playerId">Id (primary key) of target player</param>
    /// <param name="mode">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    /// <remarks>Since filter uses Any, invalid matches can still exist in the resulting query</remarks>
    /// <returns></returns>
    protected IQueryable<Tournament> QueryForParticipation(
        int playerId,
        int mode,
        DateTime? dateMin,
        DateTime? dateMax
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return _context.Tournaments
            .Include(t => t.Matches)
            .ThenInclude(m => m.MatchRatingStats)
            .Where(t =>
                t.Ruleset == (Ruleset)mode
                // Contains *any* match that is:
                && t.Matches.Any(m =>
                    // Within time range
                    m.StartTime >= dateMin
                    && m.StartTime <= dateMax
                    // Verified
                    && m.VerificationStatus == Old_MatchVerificationStatus.Verified
                    // Participated in by player
                    && m.MatchRatingStats.Any(stat => stat.PlayerId == playerId)
                ));
    }

    private IQueryable<Tournament> TournamentsBaseQuery()
    {
        return _context.Tournaments
            .Include(e => e.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.MatchScores)
            .Include(e => e.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.Beatmap)
            .Include(e => e.SubmittedByUser)
            .AsSplitQuery();
    }
}
