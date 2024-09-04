using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class PlayersRepository(OtrContext context) : RepositoryBase<Player>(context), IPlayersRepository
{
    private readonly OtrContext _context = context;

    public async Task<Player?> GetByOsuIdAsync(long osuId) =>
        LocalView.FirstOrDefault(p => p.OsuId == osuId)
        ?? await _context.Players.FirstOrDefaultAsync(p => p.OsuId == osuId);

    public async Task<IEnumerable<Player?>> GetAsync(IEnumerable<long> osuIds) =>
        await _context.Players.Where(p => osuIds.Contains(p.OsuId)).ToListAsync();

    public async Task<IEnumerable<Player>> GetAsync(bool eagerLoad)
    {
        if (eagerLoad)
        {
            return await _context
                .Players.Include(x => x.Scores)
                .Include(x => x.Ratings)
                .AsNoTracking()
                .ToListAsync();
        }

        return await _context.Players.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Player>> SearchAsync(string username) =>
        await _context.Players
            .WhereUsername(username, true)
            .Include(p => p.Ratings)
            .AsNoTracking()
            .Take(30)
            .ToListAsync();

    public async Task<Player?> GetVersatileAsync(string key, bool eagerLoad)
    {
        IQueryable<Player> query = _context.Players.AsNoTracking();
        if (eagerLoad)
        {
            query = query.Include(p => p.User).ThenInclude(u => u!.Settings);
        }

        if (!int.TryParse(key, out var value))
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
        if (long.TryParse(key, out var longValue))
        {
            return await query.FirstOrDefaultAsync(p => p.OsuId == longValue);
        }

        return await query.WhereUsername(key, false).FirstOrDefaultAsync();
    }

    public async Task<Player> GetOrCreateAsync(long osuId) =>
        await _context.Players.FirstOrDefaultAsync(x => x.OsuId == osuId)
            ?? await CreateAsync(new Player { OsuId = osuId });

    public async Task<int?> GetIdAsync(long osuId) =>
        await _context.Players.Where(p => p.OsuId == osuId).Select(p => p.Id).FirstOrDefaultAsync();

    public async Task<long> GetOsuIdAsync(int id) =>
        await _context.Players.Where(p => p.Id == id).Select(p => p.OsuId).FirstOrDefaultAsync();

    public async Task<string?> GetUsernameAsync(long osuId) =>
        await _context.Players.WhereOsuId(osuId).Select(p => p.Username).FirstOrDefaultAsync();

    public async Task<string?> GetUsernameAsync(int id) =>
        await _context.Players.Where(p => p.Id == id).Select(p => p.Username).FirstOrDefaultAsync();

    public async Task<int> GetIdAsync(int userId) =>
        await _context
            .Players.AsNoTracking()
            .Where(x => x.User != null && x.User.Id == userId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

    public async Task<string?> GetCountryAsync(int playerId) =>
        await _context.Players.Where(p => p.Id == playerId).Select(p => p.Country).FirstOrDefaultAsync();

    public async Task<int?> GetIdAsync(string username) =>
        await _context.Players
            .WhereUsername(username, false)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

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
            .OrderBy(p => p.Id)
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
            .OrderBy(p => p.Id)
            .Take(limit)
            .ToListAsync();
}
