using API.Entities;

namespace API.Services.Interfaces;

public interface IUserService
{
	// TODO: Change to DTOs
	Task<User?> GetForPlayerAsync(int playerId);
	Task<User?> GetForPlayerAsync(long osuId);
	Task<User?> GetOrCreateSystemUserAsync();
	Task<bool> HasRoleAsync(long osuId, string role);
}