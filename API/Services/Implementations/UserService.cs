using API.Entities;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class UserService : ServiceBase<User>, IUserService
{
	private readonly ILogger<UserService> _logger;
	private readonly OtrContext _context;

	public UserService(ILogger<UserService> logger, OtrContext context) : base(logger, context)
	{
		_logger = logger;
		_context = context;
	}

	public async Task<User?> GetForPlayerAsync(int playerId) => await _context.Users.FirstOrDefaultAsync(u => u.PlayerId == playerId);

	public async Task<User?> GetForPlayerAsync(long osuId) => await _context.Users
	                                                                        .AsNoTracking()
	                                                                        .FirstOrDefaultAsync(x => x.Player.OsuId == osuId);

	public async Task<User?> GetOrCreateSystemUserAsync()
	{
		var sysUser = await _context.Users.FirstOrDefaultAsync(u => u.Roles.Contains("System"));
		if (sysUser == null)
		{
			var created = await CreateAsync(new User
			{
				Roles = new[] { "System" }
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
		return await _context.Users.AnyAsync(u => u.Player.OsuId == osuId && u.Roles.Contains(role));
	}
}