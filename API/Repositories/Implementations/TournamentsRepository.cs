using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Entities;
using API.Enums;
using API.Osu.Enums;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class TournamentsRepository(OtrContext context) : RepositoryBase<Tournament>(context), ITournamentsRepository
{
    private readonly OtrContext _context = context;

    public async Task<Tournament?> GetAsync(int id, bool eagerLoad = false) =>
        eagerLoad ? await TournamentsBaseQuery().FirstOrDefaultAsync(x => x.Id == id) : await base.GetAsync(id);

    public async Task<bool> ExistsAsync(string name, int mode) =>
        await _context.Tournaments.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Mode == mode);

    public async Task<IEnumerable<TournamentSearchResultDTO>> SearchAsync(string name)
    {
        //_ is a wildcard character in psql so it needs to have an escape character added in front of it.
        name = name.Replace("_", @"\_");
        return await _context.Tournaments
            .AsNoTracking()
            .Where(x =>
                EF.Functions.ILike(x.Name, $"%{name}%", @"\")
                || EF.Functions.ILike(x.Abbreviation, $"%{name}%", @"\")
            )
            .Select(t => new TournamentSearchResultDTO()
            {
                Id = t.Id,
                Ruleset = (Ruleset)t.Mode,
                TeamSize = t.TeamSize,
                Name = t.Name
            })
            .Take(30)
            .ToListAsync();
    }

    public async Task<PlayerTournamentTeamSizeCountDTO> GetTeamSizeStatsAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var participatedTournaments =
            await QueryForParticipation(playerId, mode, dateMin, dateMax)
            .Select(t => new { TournamentId = t.Id, t.TeamSize })
            .Distinct() // Ensures each tournament is counted once
            .ToListAsync();

        return new PlayerTournamentTeamSizeCountDTO
        {
            Count1v1 = participatedTournaments.Count(x => x.TeamSize == 1),
            Count2v2 = participatedTournaments.Count(x => x.TeamSize == 2),
            Count3v3 = participatedTournaments.Count(x => x.TeamSize == 3),
            Count4v4 = participatedTournaments.Count(x => x.TeamSize == 4),
            CountOther = participatedTournaments.Count(x => x.TeamSize > 4)
        };
    }

    public async Task<IEnumerable<PlayerTournamentMatchCostDTO>> GetPerformancesAsync(int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax,
        int count,
        bool bestPerformances)
    {
        IQueryable<PlayerTournamentMatchCostDTO> query =
            QueryForParticipation(playerId, mode, dateMin, dateMax)
            .Select(t => new PlayerTournamentMatchCostDTO()
            {
                PlayerId = playerId,
                Mode = mode,
                TournamentId = t.Id,
                TournamentName = t.Name,
                TournamentAcronym = t.Abbreviation,
                // Calc average match cost
                MatchCost = t.Matches
                    // Filter invalid matches (Above filter uses Any, so invalid matches can still be included)
                    .Where(m => m.VerificationStatus == MatchVerificationStatus.Verified)
                    // Filter for ratings belonging to target player
                    .SelectMany(m => m.RatingStats)
                    .Where(mrs => mrs.PlayerId == playerId)
                    .Average(mrs => mrs.MatchCost)
            });

        // Sort
        query = bestPerformances
            ? query.OrderByDescending(d => d.MatchCost)
            : query.OrderBy(d => d.MatchCost);

        return await query.Take(count).ToListAsync();
    }

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
    private IQueryable<Tournament> QueryForParticipation(
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
            .ThenInclude(m => m.RatingStats)
            .Where(t =>
                t.Mode == mode
                // Contains *any* match that is:
                && t.Matches.Any(m =>
                    // Within time range
                    m.StartTime >= dateMin
                    && m.StartTime <= dateMax
                    // Verified
                    && m.VerificationStatus == MatchVerificationStatus.Verified
                    // Participated in by player
                    && m.RatingStats.Any(stat => stat.PlayerId == playerId)
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
            .Include(e => e.SubmittedBy)
            .AsSplitQuery();
    }
}
