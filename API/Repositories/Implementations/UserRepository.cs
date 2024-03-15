using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class UserRepository(ILogger<UserRepository> logger, OtrContext context) : RepositoryBase<User>(context), IUserRepository
{
    private readonly ILogger<UserRepository> _logger = logger;
    private readonly OtrContext _context = context;

    public override async Task<User?> GetAsync(int id)
    {
        return await UserBaseQuery()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetForPlayerAsync(int playerId)
    {
        return await _context.Users.Include(x => x.Player).FirstOrDefaultAsync(u => u.PlayerId == playerId);
    }

    public async Task<User?> GetForPlayerAsync(long osuId) =>
        await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Player.OsuId == osuId);

    public async Task<User?> GetOrCreateSystemUserAsync()
    {
        User? sysUser = await _context.Users.FirstOrDefaultAsync(u => u.Scopes.Contains("System"));
        if (sysUser == null)
        {
            return await CreateAsync(new User { Scopes = ["System"] });
        }
        return sysUser;
    }

    public async Task<bool> HasRoleAsync(long osuId, string role)
    {
        return await _context.Users.AnyAsync(u => u.Player.OsuId == osuId && u.Scopes.Contains(role));
    }

    public async Task<User> GetOrCreateAsync(int playerId)
    {
        if (await _context.Users.AnyAsync(x => x.PlayerId == playerId))
        {
            return await _context.Users.FirstAsync(x => x.PlayerId == playerId);
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

    private IQueryable<User> UserBaseQuery() =>
        _context.Users
            .Include(x => x.Player)
            .Include(x => x.RateLimitOverrides);
}
