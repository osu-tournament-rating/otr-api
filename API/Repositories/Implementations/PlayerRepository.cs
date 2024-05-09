using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Entities;
using API.Handlers.Interfaces;
using API.Osu.Enums;
using API.Repositories.Interfaces;
using API.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class PlayerRepository(OtrContext context, IMapper mapper, ICacheHandler cacheHandler) : RepositoryBase<Player>(context), IPlayerRepository, IUsesCache
{
    private readonly OtrContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheHandler _cacheHandler = cacheHandler;

    public async Task InvalidateCacheEntriesAsync()
    {
        await _cacheHandler.OnPlayerUpdateAsync();
    }

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
        await SearchQuery(username, true)
            .Include(p => p.Ratings)
            .AsNoTracking()
            .Take(30)
            .ToListAsync();

    public async Task<Player?> GetAsync(string username)
    {
        List<Player> players = await SearchQuery(username, false).ToListAsync();

        if (players.Count > 1)
        {
            throw new Exception("More than one player was found.");
        }

        return players.Count == 0 ? null : players.First();
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

    public async Task<IEnumerable<PlayerRatingDTO>> GetTopRatingsAsync(int n, Ruleset ruleset) => await (
            from p in _context.Players
            join r in _context.BaseStats on p.Id equals r.PlayerId
            where r.Mode == ruleset
            orderby r.Rating descending
            select new PlayerRatingDTO
            {
                OsuId = p.OsuId,
                Username = p.Username ?? "User" + p.Id,
                Mu = r.Rating,
                Sigma = r.Volatility
            }
        ).Take(n).ToListAsync();

    public async Task<string?> GetUsernameAsync(long osuId) =>
        await _context.Players.WhereOsuId(osuId).Select(p => p.Username).FirstOrDefaultAsync();

    public async Task<string?> GetUsernameAsync(int id) =>
        await _context.Players.Where(p => p.Id == id).Select(p => p.Username).FirstOrDefaultAsync();

    public async Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync() =>
        (await _context.Players.AsNoTracking().ToDictionaryAsync(p => p.OsuId, p => p.Id))
            .OrderBy(x => x.Value)
            .Select(x => new PlayerIdMappingDTO { Id = x.Value, OsuId = x.Key });

    public async Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync() =>
        await _context
            .Players.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new PlayerCountryMappingDTO { PlayerId = x.Id, Country = x.Country })
            .ToListAsync();

    public async Task<int> GetIdAsync(int userId) =>
        await _context
            .Players.AsNoTracking()
            .Where(x => x.User != null && x.User.Id == userId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

    public async Task<string?> GetCountryAsync(int playerId) =>
        await _context.Players.Where(p => p.Id == playerId).Select(p => p.Country).FirstOrDefaultAsync();

    public async Task<int?> GetIdAsync(string username)
    {
        return await SearchQuery(username, false).Select(x => x.Id).FirstOrDefaultAsync();
    }

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

        if (partialMatch)
        {
            return _context.Players.Where(p => p.Username != null && EF.Functions.ILike(p.Username ?? string.Empty, $"%{username}%", @"\"));
        }

        return _context.Players.Where(p => p.Username != null && EF.Functions.ILike(p.Username ?? string.Empty, username, @"\"));
    }

    // This is used by a scheduled task to automatically populate user info, such as username, country, etc.
    public async Task<IEnumerable<Player>> GetOutdatedAsync() =>
        await _context
            .Players.Where(p => p.Updated == null || (DateTime.UtcNow - p.Updated) > TimeSpan.FromDays(14))
            .ToListAsync();

    public async Task<PlayerInfoDTO?> GetPlayerDTOByOsuIdAsync(
        long osuId,
        bool eagerLoad = false,
        Ruleset ruleset = Ruleset.Standard,
        int offsetDays = -1
    )
    {
        PlayerInfoDTO? obj = _mapper.Map<PlayerInfoDTO?>(await GetAsync(osuId, eagerLoad, (int)ruleset, offsetDays));

        if (obj == null)
        {
            return obj;
        }

        return obj;
    }
}
