using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

/// <summary>
/// Repository for managing <see cref="Player"/> entities
/// </summary>
public class PlayersRepository(OtrContext context) : Repository<Player>(context), IPlayersRepository
{
    private readonly OtrContext _context = context;

    public async Task<ICollection<Player>> GetAsync(IEnumerable<int> ids) =>
        await _context.Players
            .Include(p => p.User)
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id)).ToListAsync();

    public async Task<IEnumerable<Player>> GetAsync(IEnumerable<long> osuIds) =>
        await _context.Players.AsNoTracking().Where(p => osuIds.Contains(p.OsuId)).ToListAsync();

    public async Task<IEnumerable<Player>> SearchAsync(string username) =>
        await _context.Players
            .AsNoTracking()
            .WhereUsername(username, true)
            .Include(p => p.Ratings)
            .Include(p => p.User)
            .ThenInclude(u => u!.Settings)
            .OrderBy(p => p.Username)
            .Take(30)
            .ToListAsync();

    public async Task<Player?> GetVersatileAsync(string key, bool eagerLoad)
    {
        IQueryable<Player> query = _context.Players.AsNoTracking();
        if (eagerLoad)
        {
            query = query.Include(p => p.User).ThenInclude(u => u!.Settings);
        }

        if (!int.TryParse(key, out int value))
        {
            return await query.WhereUsername(key, false).FirstOrDefaultAsync();
        }

        // Check for the player id
        Player? result = await query.FirstOrDefaultAsync(p => p.Id == value);

        if (result != null)
        {
            return result;
        }

        // Check for the osu id
        if (long.TryParse(key, out long longValue))
        {
            return await query.FirstOrDefaultAsync(p => p.OsuId == longValue);
        }

        return await query.WhereUsername(key, false).FirstOrDefaultAsync();
    }

    public async Task<Player> GetOrCreateAsync(long osuId) =>
        await _context.Players.FirstOrDefaultAsync(x => x.OsuId == osuId)
        ?? await CreateAsync(new Player { OsuId = osuId });

    public async Task SetOutdatedOsuAsync(TimeSpan outdatedAfter)
    {
        await _context.Players
            .Where(p => DateTime.UtcNow - p.OsuLastFetch < outdatedAfter)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(p => p.OsuLastFetch, DateTime.UtcNow - outdatedAfter));
    }

    public async Task<IEnumerable<Player>> GetOutdatedOsuAsync(TimeSpan outdatedAfter, int limit) =>
        await _context.Players
            .Include(p => p.RulesetData)
            .Include(p => p.User)
            .ThenInclude(u => u!.Settings)
            .Where(p => DateTime.UtcNow - p.OsuLastFetch > outdatedAfter)
            .OrderBy(p => p.OsuLastFetch)
            .Take(limit)
            .ToListAsync();

    public async Task SetOutdatedOsuTrackAsync(TimeSpan outdatedAfter)
    {
        await _context.Players
            .Where(p => DateTime.UtcNow - p.OsuTrackLastFetch < outdatedAfter)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(p => p.OsuTrackLastFetch, DateTime.UtcNow - outdatedAfter));
    }

    public async Task<IEnumerable<Player>> GetOutdatedOsuTrackAsync(TimeSpan outdatedAfter, int limit) =>
        await _context.Players
            .Include(p => p.RulesetData)
            .Include(p => p.MatchStats)
            .ThenInclude(pms => pms.Match)
            .Where(p => DateTime.UtcNow - p.OsuTrackLastFetch > outdatedAfter)
            .OrderBy(p => p.OsuTrackLastFetch)
            .Take(limit)
            .ToListAsync();
}
