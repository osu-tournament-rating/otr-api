using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
	private readonly ILogger<UserRepository> _logger;
	private readonly OtrContext _context;

	public UserRepository(ILogger<UserRepository> logger, OtrContext context) : base(context)
	{
		_logger = logger;
		_context = context;
	}

	public async Task<User?> GetForPlayerAsync(int playerId)
	{
		return await _context.Users
		                     .Include(x => x.Player)
		                     .FirstOrDefaultAsync(u => u.PlayerId == playerId);
	}

	public async Task<User?> GetForPlayerAsync(long osuId) => await _context.Users
	                                                                        .AsNoTracking()
	                                                                        .FirstOrDefaultAsync(x => x.Player.OsuId == osuId);

	public async Task<User?> GetOrCreateSystemUserAsync()
	{
		var sysUser = await _context.Users.FirstOrDefaultAsync(u => u.Scopes.Contains("System"));
		if (sysUser == null)
		{
			var created = await CreateAsync(new User
			{
				Scopes = new[] { "System" }
			});

			if (created == null)
			{
				_logger.LogError("Failed to create system user");
				return null;
			}

			return created;
		}

		return sysUser;
	}

	public async Task<bool> HasRoleAsync(long osuId, string role)
	{
		return await _context.Users.AnyAsync(u => u.Player.OsuId == osuId && u.Scopes.Contains(role));
	}
}