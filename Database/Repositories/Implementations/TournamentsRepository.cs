using System.Diagnostics.CodeAnalysis;
using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;
using Database.Entities.Processor;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class TournamentsRepository(OtrContext context, IBeatmapsRepository beatmapsRepository)
    : RepositoryBase<Tournament>(context), ITournamentsRepository
{
    private readonly OtrContext _context = context;

    public async Task<Tournament?> GetAsync(int id, bool eagerLoad = false) =>
        eagerLoad
            ? await TournamentsBaseQuery().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
            : await base.GetAsync(id);

    public async Task<Tournament?> GetVerifiedAsync(int id) =>
        await _context.Tournaments
            .AsNoTracking()
            .AsSplitQuery()
            .Include(t => t.Matches.Where(m =>
                m.VerificationStatus == VerificationStatus.Verified &&
                m.ProcessingStatus == MatchProcessingStatus.Done))
            .ThenInclude(m => m.Games.Where(g =>
                g.VerificationStatus == VerificationStatus.Verified && g.ProcessingStatus == GameProcessingStatus.Done))
            .ThenInclude(g => g.Beatmap)
            .Include(t => t.Matches.Where(m =>
                m.VerificationStatus == VerificationStatus.Verified &&
                m.ProcessingStatus == MatchProcessingStatus.Done))
            .ThenInclude(m => m.Games.Where(g =>
                g.VerificationStatus == VerificationStatus.Verified && g.ProcessingStatus == GameProcessingStatus.Done))
            .ThenInclude(g => g.Scores.Where(gs =>
                gs.VerificationStatus == VerificationStatus.Verified &&
                gs.ProcessingStatus == ScoreProcessingStatus.Done))
            .ThenInclude(gs => gs.Player)
            .Include(t => t.SubmittedByUser!.Player)
            .Include(t => t.VerifiedByUser!.Player)
            .Include(t => t.AdminNotes)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Tournament>> GetNeedingProcessingAsync(int limit) =>
        await _context.Tournaments
            .AsSplitQuery()
            .Include(t => t.PooledBeatmaps)
            .Include(t => t.PlayerTournamentStats)
            .Include(t => t.Matches)
            .ThenInclude(m => m.Rosters)
            .Include(t => t.Matches)
            .ThenInclude(m => m.PlayerMatchStats)
            .ThenInclude(pms => pms.Player)
            .Include(t => t.Matches)
            .ThenInclude(m => m.PlayerRatingAdjustments)
            .Include(t => t.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.Beatmap)
            .Include(t => t.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.Scores)
            .ThenInclude(s => s.Player)
            .Include(t => t.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.Rosters)
            .Where(t => t.ProcessingStatus != TournamentProcessingStatus.Done &&
                        t.ProcessingStatus != TournamentProcessingStatus.NeedsApproval)
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

    public async Task<ICollection<Tournament>> GetAsync(
        int page,
        int pageSize,
        TournamentQuerySortType querySortType,
        bool verified = true,
        Ruleset? ruleset = null,
        string? searchQuery = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        VerificationStatus? verificationStatus = null,
        TournamentRejectionReason? rejectionReason = null,
        TournamentProcessingStatus? processingStatus = null,
        int? submittedBy = null,
        int? verifiedBy = null,
        int? lobbySize = null,
        bool descending = true
    )
    {
        IQueryable<Tournament> query = _context.Tournaments
            .AsNoTracking()
            .Include(t => t.SubmittedByUser!.Player)
            .Include(t => t.VerifiedByUser!.Player)
            .WhereRuleset(ruleset)
            .WhereSearchQuery(searchQuery)
            .WhereDateRange(dateMin, dateMax)
            .WhereVerificationStatus(verificationStatus)
            .WhereRejectionReason(rejectionReason)
            .WhereProcessingStatus(processingStatus)
            .WhereSubmittedBy(submittedBy)
            .WhereVerifiedBy(verifiedBy)
            .WhereLobbySize(lobbySize);

        if (verified)
        {
            query = query
                .WhereVerificationStatus(VerificationStatus.Verified)
                .WhereProcessingStatus(TournamentProcessingStatus.Done);
        }

        return await query
            .OrderBy(querySortType, descending)
            .Page(pageSize, page - 1)
            .ToListAsync();
    }

    public async Task<Tournament?> AcceptPreVerificationStatusesAsync(int id, int verifierUserId)
    {
        Tournament? tournament = await TournamentsBaseQuery()
            .Where(t => t.ProcessingStatus == TournamentProcessingStatus.NeedsVerification)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tournament is null)
        {
            return null;
        }

        #region Confirm "pre" verification statuses

        tournament.ConfirmPreVerificationStatus();
        tournament.VerifiedByUserId = verifierUserId;

        foreach (Match match in tournament.Matches)
        {
            match.ConfirmPreVerificationStatus();
            match.VerifiedByUserId = verifierUserId;

            foreach (Game game in match.Games)
            {
                game.ConfirmPreVerificationStatus();

                foreach (GameScore score in game.Scores)
                {
                    score.ConfirmPreVerificationStatus();
                }
            }
        }

        #endregion

        await UpdateAsync(tournament);
        return tournament;
    }

    public async Task<ICollection<Beatmap>> GetPooledBeatmapsAsync(int id) =>
        (await _context.Tournaments.Include(t => t.PooledBeatmaps)
            .FirstOrDefaultAsync(t => t.Id == id))?.PooledBeatmaps ?? [];

    public async Task<Dictionary<int, int>> GetLobbySizeStatsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var participatedTournaments =
            await QueryForParticipation(playerId, ruleset, dateMin, dateMax)
                .Select(t => new { TournamentId = t.Id, TeamSize = t.LobbySize })
                .Distinct() // Ensures each tournament is counted once
                .ToListAsync();

        // Group by team size and count occurrences
        var lobbySizeCounts = participatedTournaments
            .GroupBy(t => t.TeamSize)
            .ToDictionary(g => g.Key, g => g.Count());

        // Ensure all team sizes are represented, even if count is zero
        var result = new Dictionary<int, int>
        {
            { 1, lobbySizeCounts.GetValueOrDefault(1, 0) },
            { 2, lobbySizeCounts.GetValueOrDefault(2, 0) },
            { 3, lobbySizeCounts.GetValueOrDefault(3, 0) },
            { 4, lobbySizeCounts.GetValueOrDefault(4, 0) },
            { -1, lobbySizeCounts.Where(kvp => kvp.Key > 4).Sum(kvp => kvp.Value) } // "Other" category
        };

        return result;
    }

    public async Task<IList<Tournament>> SearchAsync(string name) =>
        await _context.Tournaments
            .AsNoTracking()
            .WhereSearchQuery(name)
            .Take(30)
            .ToListAsync();

    public async Task<ICollection<Beatmap>> AddPooledBeatmapsAsync(int id, ICollection<long> osuBeatmapIds)
    {
        Tournament? tournament = await _context.Tournaments
            .Include(t => t.PooledBeatmaps)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tournament is null)
        {
            return [];
        }

        IEnumerable<int> existingIds = tournament.PooledBeatmaps.Select(b => b.Id);
        ICollection<Beatmap> beatmaps = await beatmapsRepository.GetOrCreateAsync(osuBeatmapIds, save: false);

        var unmappedBeatmaps = beatmaps.ExceptBy(existingIds, b => b.Id).ToList();
        unmappedBeatmaps.ForEach(b => tournament.PooledBeatmaps.Add(b));

        await UpdateAsync(tournament);

        return tournament.PooledBeatmaps;
    }

    public async Task DeletePooledBeatmapsAsync(int id)
    {
        Tournament? tournament = await _context.Tournaments
            .Include(t => t.PooledBeatmaps)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tournament is null || tournament.PooledBeatmaps.Count == 0)
        {
            return;
        }

        tournament.PooledBeatmaps = [];
        await UpdateAsync(tournament);
    }

    public async Task DeletePooledBeatmapsAsync(int id, ICollection<int> beatmapIds)
    {
        Tournament? tournament = await _context.Tournaments
            .Include(t => t.PooledBeatmaps)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tournament is null)
        {
            return;
        }

        var beatmaps = tournament.PooledBeatmaps.Where(b => beatmapIds.Contains(b.Id)).ToList();
        if (beatmaps.Count == 0)
        {
            return;
        }

        beatmaps.ForEach(b => tournament.PooledBeatmaps.Remove(b));
        await UpdateAsync(tournament);
    }

    public async Task ResetAutomationStatusesAsync(int id, bool force = false)
    {
        Tournament? tournament = await TournamentsBaseQuery()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tournament is null)
        {
            return;
        }

        tournament.ResetAutomationStatuses(force);
        foreach (Match match in tournament.Matches)
        {
            match.ResetAutomationStatuses(force);
            foreach (Game game in match.Games)
            {
                game.ResetAutomationStatuses(force);

                foreach (GameScore score in game.Scores)
                {
                    score.ResetAutomationStatuses(force);
                }
            }
        }

        await UpdateAsync(tournament);
    }

    /// <summary>
    /// Returns a queryable containing tournaments for <see cref="ruleset"/>
    /// with *any* match applicable to all the following criteria:
    /// - Is verified
    /// - Started between <paramref name="dateMin"/> and <paramref name="dateMax"/>
    /// - Contains a <see cref="RatingAdjustment"/> for given <paramref name="playerId"/> (Denotes participation)
    /// </summary>
    /// <param name="playerId">Primary key of target player</param>
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
            .Include(t => t.SubmittedByUser)
            .Include(t => t.VerifiedByUser)
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
            .ThenInclude(b => b!.Beatmapset)
            .ThenInclude(bs => bs!.Creator)
            .Include(e => e.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.Beatmap)
            .ThenInclude(b => b!.Creators)
            .Include(e => e.SubmittedByUser)
            .Include(t => t.AdminNotes)
            .Include(t => t.PooledBeatmaps)
            .Include(t => t.SubmittedByUser!.Player)
            .Include(t => t.VerifiedByUser!.Player)
            .AsSplitQuery();
    }
}
