using Common.Enums.Enums;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class PlayerTournamentStatsRepository(OtrContext context) : RepositoryBase<PlayerTournamentStats>(context), IPlayerTournamentStatsRepository
{
    private readonly OtrContext _context = context;

    public async Task<IDictionary<int, (int sumTournaments, int sumMatches, double averageMatchWinRate)>> GetLeaderboardStatsAsync(
        IEnumerable<int> playerIds,
        Ruleset ruleset) => await _context.PlayerTournamentStats
        .AsNoTracking()
        .Where(pts => playerIds.Contains(pts.PlayerId))
        .Where(pts => pts.Tournament.Ruleset == ruleset)
        .GroupBy(pts => pts.PlayerId)
        .Select(grouping => new
        {
            PlayerId = grouping.Key,
            SumTournaments = grouping.Count(),
            SumMatches = grouping.Sum(pts => pts.MatchesPlayed),
            AverageWinRate = grouping.Average(pts => pts.MatchWinRate),
            TotalMatches = grouping.Sum(pts => pts.MatchesPlayed)
        })
        .ToDictionaryAsync(
            x => x.PlayerId,
            x => (
                x.SumTournaments,
                x.SumMatches,
                x.AverageWinRate
            )
        );
    public async Task<ICollection<PlayerTournamentStats>> GetForPlayerAsync(int playerId, Ruleset ruleset, DateTime? dateMin, DateTime? dateMax) =>
        await _context.PlayerTournamentStats
            .Include(pts => pts.Player)
            .Include(pts => pts.Tournament)
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(pts => pts.PlayerId == playerId)
            .ToListAsync();

    public async Task<ICollection<PlayerTournamentStats>> GetBestPerformancesAsync(int playerId, int count, Ruleset ruleset,
        DateTime? dateMin, DateTime? dateMax) =>
         await _context.PlayerTournamentStats
            .Include(pts => pts.Player)
            .Include(pts => pts.Tournament)
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(pts => pts.PlayerId == playerId)
            .OrderByDescending(pts => pts.AverageMatchCost)
            .Take(count)
            .ToListAsync();

    public async Task<ICollection<PlayerTournamentStats>> GetRecentPerformancesAsync(int playerId, int count,
        Ruleset ruleset, DateTime? dateMin, DateTime? dateMax) =>
        await _context.PlayerTournamentStats
            .Include(pts => pts.Player.User)
            .Include(pts => pts.Tournament.SubmittedByUser!.Player)
            .Include(pts => pts.Tournament.VerifiedByUser!.Player)
            .ApplyCommonFilters(ruleset, dateMin, dateMax)
            .Where(pts => pts.PlayerId == playerId)
            .OrderByDescending(pts => pts.Tournament.StartTime)
            .Take(count)
            .ToListAsync();
}
