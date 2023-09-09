using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class UserService : ServiceBase<User>, IUserService
{
	public UserService(ILogger<UserService> logger) : base(logger) {}

	public async Task<User?> GetForPlayerAsync(int playerId)
	{
		using (var context = new OtrContext())
		{
			return await context.Users.FirstOrDefaultAsync(u => u.PlayerId == playerId);
		}
	}
}