using System.Diagnostics.CodeAnalysis;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class PlayerRepository(OtrContext context) : RepositoryBase<Player>(context), IPlayerRepository
{
    private readonly OtrContext _context = context;

    public override async Task<int> UpdateAsync(Player player)
    {
        player.Updated = DateTime.UtcNow;
        return await base.UpdateAsync(player);
    }

    public async Task<IEnumerable<Player>> SearchAsync(string username) =>
        await SearchQuery(username, true).ToListAsync();

    public async Task<Player?> GetVersatileAsync(string key)
    {
        if (!int.TryParse(key, out var value))
        {
            return await GetAsync(key);
        }

        // Check for the player id
        Player? result = await GetAsync(id: value);

        if (result != null)
        {
            return result;
        }

        // Check for the osu id
        if (long.TryParse(key, out var longValue))
        {
            return await GetAsync(longValue);
        }

        return await GetAsync(key);
    }

    public async Task<Player?> GetAsync(string username) =>
        await SearchQuery(username, false).FirstOrDefaultAsync();

    public async Task<Player?> GetAsync(long osuId) =>
        await _context.Players.FirstOrDefaultAsync(p => p.OsuId == osuId);

    public async Task<Player> GetOrCreateAsync(long osuId) =>
        await GetAsync(osuId) ?? await CreateAsync(new Player { OsuId = osuId });

    public async Task<int?> GetIdAsync(string username) =>
        await SearchQuery(username, false).Select(x => x.Id).FirstOrDefaultAsync();

    public async Task<int?> GetIdAsync(long osuId) =>
        await _context.Players.Where(p => p.OsuId == osuId).Select(p => p.Id).FirstOrDefaultAsync();

    // TODO: Refactor to return Task<long?>
    public async Task<long> GetOsuIdAsync(int id) =>
        await _context.Players.Where(p => p.Id == id).Select(p => p.OsuId).FirstOrDefaultAsync();

    public async Task<string?> GetUsernameAsync(int id) =>
        await _context.Players.Where(p => p.Id == id).Select(p => p.Username).FirstOrDefaultAsync();

    public async Task<string?> GetCountryAsync(int id) =>
        await _context.Players.Where(p => p.Id == id).Select(p => p.Country).FirstOrDefaultAsync();

    public async Task<IEnumerable<Player>> GetMissingRankAsync()
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

    public async Task<IEnumerable<Player>> GetOutdatedAsync() =>
        await _context
            .Players.Where(p => p.Updated == null || (DateTime.UtcNow - p.Updated) > TimeSpan.FromDays(14))
            .ToListAsync();

    /// <summary>
    /// Helper method for searching by username consistently
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <param name="partialMatch">Whether or not we want to check for partial name matches on lookup.</param>
    /// <returns>A query that filters players by the username provided</returns>
    private IQueryable<Player> SearchQuery(string username, bool partialMatch)
    {
        //_ is a wildcard character in psql so it needs to have an escape character added in front of it.
        username = username.Replace("_", @"\_");
        var pattern = partialMatch ? $"%{username}%" : username;

        return _context.Players.Where(p => p.Username != null && EF.Functions.ILike(p.Username ?? string.Empty, pattern, @"\"));
    }
}
