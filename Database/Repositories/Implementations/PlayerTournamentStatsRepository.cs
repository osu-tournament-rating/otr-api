using Common.Enums;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class PlayerTournamentStatsRepository(OtrContext context) : RepositoryBase<PlayerTournamentStats>(context), IPlayerTournamentStatsRepository
{
    private readonly OtrContext _context = context;

    public async Task<IDictionary<int, IList<PlayerTournamentStats>>> GetAsync(IEnumerable<int> playerIds, Ruleset ruleset) =>
         await _context.PlayerTournamentStats
            .Where(pts => playerIds.Contains(pts.PlayerId) && pts.Tournament.Ruleset == ruleset)
        .GroupBy(pts => pts.PlayerId)
        .ToDictionaryAsync(g => g.Key, IList<PlayerTournamentStats> (g) => g.ToList());

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
