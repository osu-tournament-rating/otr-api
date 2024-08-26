using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Entities.Processor;
using Database.Enums;
using Database.Enums.Verification;
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

    public async Task<IEnumerable<Tournament>> GetNeedingProcessingAsync(int limit) =>
        await _context.Tournaments
            .AsSplitQuery()
            .Include(t => t.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.Beatmap)
            .Include(t => t.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.Scores)
            .ThenInclude(s => s.Player)
            .Where(t => t.ProcessingStatus != TournamentProcessingStatus.Done)
            .OrderBy(t => t.LastProcessingDate)
            .Take(limit)
            .ToListAsync();

    public async Task<bool> ExistsAsync(string name, Ruleset ruleset) =>
        await _context.Tournaments.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Ruleset == ruleset);

    public async Task<int> CountPlayedAsync(
        int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    ) => await QueryForParticipation(playerId, ruleset, dateMin, dateMax).Select(x => x.Id).Distinct().CountAsync();

    /// <summary>
    /// Returns a queryable containing tournaments for <see cref="ruleset"/>
    /// with *any* match applicable to all of the following criteria:
    /// - Is verified
    /// - Started between <paramref name="dateMin"/> and <paramref name="dateMax"/>
    /// - Contains a <see cref="RatingAdjustment"/> for given <paramref name="playerId"/> (Denotes participation)
    /// </summary>
    /// <param name="playerId">Id (primary key) of target player</param>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    /// <remarks>Since filter uses Any, invalid matches can still exist in the resulting query</remarks>
    /// <returns></returns>
    protected IQueryable<Tournament> QueryForParticipation(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin,
        DateTime? dateMax
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return _context.Tournaments
            .Include(t => t.Matches)
            .ThenInclude(m => m.PlayerRatingAdjustments)
            .Where(t =>
                t.Ruleset == ruleset
                // Contains *any* match that is:
                && t.Matches.Any(m =>
                    // Within time range
                    m.StartTime >= dateMin
                    && m.StartTime <= dateMax
                    // Verified
                    && m.VerificationStatus == VerificationStatus.Verified
                    // Participated in by player
                    && m.PlayerRatingAdjustments.Any(stat => stat.PlayerId == playerId)
                ));
    }

    private IQueryable<Tournament> TournamentsBaseQuery()
    {
        return _context.Tournaments
            .Include(e => e.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.Scores)
            .Include(e => e.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.Beatmap)
            .Include(e => e.SubmittedByUser)
            .AsSplitQuery();
    }
}
