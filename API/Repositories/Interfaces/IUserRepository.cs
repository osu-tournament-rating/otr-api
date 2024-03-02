using API.Entities;

namespace API.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
	Task<User?> GetForPlayerAsync(int playerId);
	Task<User?> GetForPlayerAsync(long osuId);
	Task<User?> GetOrCreateSystemUserAsync();
	Task<bool> HasRoleAsync(long osuId, string role);
	Task<User> GetOrCreateAsync(int playerId);
}