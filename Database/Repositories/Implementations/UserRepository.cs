using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance",
    "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class UserRepository(
    OtrContext context,
    ILogger<UserRepository> logger,
    IUserSettingsRepository userSettingsRepository) : RepositoryBase<User>(context), IUserRepository
{
    private readonly OtrContext _context = context;

    public override async Task<User> CreateAsync(User entity)
    {
        entity.Settings = await userSettingsRepository.GenerateDefaultAsync(entity.PlayerId);
        return await base.CreateAsync(entity);
    }

    public override async Task<User?> GetAsync(int id) =>
        await UserBaseQuery().FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByPlayerIdAsync(int playerId, bool loadSettings = false)
    {
        IQueryable<User> query = _context.Users;
        query = loadSettings
            ? query.Include(u => u.Settings)
            : query;

        return await query.FirstOrDefaultAsync(u => u.PlayerId == playerId);
    }

    public async Task<User> GetOrCreateByPlayerIdAsync(int playerId)
    {
        User? user = await GetByPlayerIdAsync(playerId);
        if (user is not null)
        {
            return user;
        }

        return await CreateAsync(new User { PlayerId = playerId });
    }

    public async Task<int?> GetPlayerIdAsync(int id) =>
        await _context.Users.Where(u => u.Id == id).Select(u => u.PlayerId).FirstOrDefaultAsync();

    public async Task<IEnumerable<OAuthClient>> GetClientsAsync(int id) =>
        await _context.Users.Where(u => u.Id == id).Select(u => u.Clients).FirstOrDefaultAsync()
        ?? [];

    public async Task<IEnumerable<Match>> GetSubmissionsAsync(int id) =>
        await _context.Users.Where(u => u.Id == id).Select(u => u.SubmittedMatches).FirstOrDefaultAsync()
        ?? [];

    public async Task<User?> SyncFriendsAsync(int id, ICollection<long> playerOsuIds)
    {
        User? user = await _context.Users
            .Include(u => u.Friends)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return null;
        }

        var idSet = playerOsuIds.ToHashSet();

        // Identify all players which we already have
        List<Player> players = await _context.Players
            .Where(p => idSet.Contains(p.OsuId))
            .ToListAsync();

        // Effectively transforms "idSet" into a "missingIds" set
        idSet.ExceptWith(players.Select(p => p.OsuId));

        // Create players and assign them to the friends list
        players.AddRange(idSet.Select(osuId => new Player { OsuId = osuId }));

        // Overwrite friends list and update
        user.Friends = players;
        user.LastFriendsListUpdate = DateTime.UtcNow;
        await UpdateAsync(user);

        logger.LogDebug("Synced {Count} friends for user {User}", user.Friends.Count, user.Id);

        return user;
    }

    private IQueryable<User> UserBaseQuery() =>
        _context.Users
            .Include(x => x.Settings)
            .Include(x => x.Player);
}
