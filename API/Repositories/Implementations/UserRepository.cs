using System.Diagnostics.CodeAnalysis;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class UserRepository(OtrContext context, IUserSettingsRepository userSettingsRepository) : RepositoryBase<User>(context), IUserRepository
{
    private readonly OtrContext _context = context;

    public override async Task<User> CreateAsync(User entity)
    {
        if (!entity.PlayerId.HasValue)
        {
            throw new NullReferenceException("Attempting to create a User entity without a PlayerId");
        }

        entity.Settings = await userSettingsRepository.GenerateDefaultAsync(entity.PlayerId.Value);
        return await base.CreateAsync(entity);
    }

    public override async Task<User?> GetAsync(int id) =>
        await UserBaseQuery().FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByPlayerIdAsync(int playerId, bool loadSettings = false)
    {
        IQueryable<User> query = _context.Users.AsNoTracking();
        query = loadSettings
            ? query.Include(u => u.Settings)
            : query;

        return await query.FirstOrDefaultAsync(u => u.PlayerId == playerId);
    }

    public async Task<User> GetByPlayerIdOrCreateAsync(int playerId)
    {
        User? user = await GetByPlayerIdAsync(playerId);
        if (user is not null)
        {
            return user;
        }

        return await CreateAsync(
            new User
            {
                PlayerId = playerId,
                Created = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Scopes = []
            }
        );
    }

    public async Task<IEnumerable<OAuthClient>> GetClientsAsync(int id) =>
        (await _context.Users
            .Include(u => u.Clients)
            .FirstOrDefaultAsync(u => u.Id == id))?.Clients ?? new List<OAuthClient>();

    public async Task<IEnumerable<Match>> GetSubmissionsAsync(int id) =>
        (await _context.Users
            .Include(u => u.SubmittedMatches)
            .FirstOrDefaultAsync(u => u.Id == id))?.SubmittedMatches ?? new List<Match>();

    private IQueryable<User> UserBaseQuery() =>
        _context.Users
            .Include(x => x.Settings)
            .Include(x => x.Player);
}
