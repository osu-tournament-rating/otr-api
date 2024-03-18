using System.Diagnostics.CodeAnalysis;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class UserRepository(OtrContext context) : RepositoryBase<User>(context), IUserRepository
{
    private readonly OtrContext _context = context;

    public override async Task<User?> GetAsync(int id) =>
        await UserBaseQuery().FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByOsuIdAsync(int osuId) =>
        await UserBaseQuery().FirstOrDefaultAsync(u => u.PlayerId == osuId);

    public async Task<User> GetOrCreateAsync(int osuId)
    {
        User? user = await GetByOsuIdAsync(osuId);
        if (user is not null)
        {
            return user;
        }

        return await CreateAsync(
            new User
            {
                PlayerId = osuId,
                Created = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Scopes = []
            }
        );
    }

    private IQueryable<User> UserBaseQuery() =>
        _context.Users
            .Include(x => x.Player);
}
