using Database.Entities;
using Database.Enums;
using Database.Extensions;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class PlayersRepository(OtrContext context) : RepositoryBase<Player>(context), IPlayersRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<Player>> GetPlayersMissingRankAsync()
    {
        // Get all players that are missing an earliest global rank in any mode (but have a current rank in that mode)
        List<Player> players = await _context
            .Players.Where(x =>
                (x.EarliestOsuGlobalRank == null && x.RankStandard != null)
                || (x.EarliestTaikoGlobalRank == null && x.RankTaiko != null)
                || (x.EarliestCatchGlobalRank == null && x.RankCatch != null)
                || (x.EarliestManiaGlobalRank == null && x.RankMania != null)
            )
            .ToListAsync();

        return players;
    }

    public async Task<IEnumerable<Player?>> GetAsync(IEnumerable<long> osuIds) =>
        await _context.Players.Where(p => osuIds.Contains(p.OsuId)).ToListAsync();

    public async Task<IEnumerable<Player>> GetAsync(bool eagerLoad)
    {
        if (eagerLoad)
        {
            return await _context
                .Players.Include(x => x.MatchScores)
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

    public async Task<Player> GetOrCreateAsync(long osuId)
    {
        if (await _context.Players.AnyAsync(x => x.OsuId == osuId))
        {
            return await _context.Players.FirstAsync(x => x.OsuId == osuId);
        }

        return await CreateAsync(new Player { OsuId = osuId });
    }

    // TODO: Refactor param "mode" to use OsuEnums.Ruleset
    public async Task<Player?> GetAsync(long osuId, bool eagerLoad, int mode = 0, int offsetDays = -1)
    {
        if (!eagerLoad)
        {
            return await _context.Players.WhereOsuId(osuId).FirstOrDefaultAsync();
        }

        DateTime time = offsetDays == -1 ? DateTime.MinValue : DateTime.UtcNow.AddDays(-offsetDays);

        Player? p = await _context
            .Players.Include(x =>
                x.MatchScores.Where(y => y.Game.StartTime > time && y.Game.Ruleset == (Ruleset)mode)
            )
            .ThenInclude(x => x.Game)
            .ThenInclude(x => x.Match)
            .Include(x => x.Ratings.Where(y => y.Mode == (Ruleset)mode))
            .Include(x => x.User)
            .WhereOsuId(osuId)
            .FirstOrDefaultAsync();

        return p ?? null;
    }

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

    // This is used by a scheduled task to automatically populate user info, such as username, country, etc.
    public async Task<IEnumerable<Player>> GetOutdatedAsync() =>
        await _context
            .Players.Where(p => p.Updated == null || (DateTime.UtcNow - p.Updated) > TimeSpan.FromDays(14))
            .ToListAsync();
}
