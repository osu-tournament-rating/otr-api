using API.Entities;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class UserService : ServiceBase<User>, IUserService
{
	private readonly OtrContext _context;

	public UserService(ILogger<UserService> logger, OtrContext context) : base(logger, context)
	{
		_context = context;
	}

	public async Task<User?> GetForPlayerAsync(int playerId) => await _context.Users.FirstOrDefaultAsync(u => u.PlayerId == playerId);
}