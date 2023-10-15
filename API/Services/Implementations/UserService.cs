using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class UserService : IUserService
{
	private readonly IUserRepository _repository;

	public UserService(IUserRepository repository) { _repository = repository; }

	public async Task<User?> GetForPlayerAsync(int playerId) => await _repository.GetForPlayerAsync(playerId);
	public async Task<User?> GetForPlayerAsync(long osuId) => await _repository.GetForPlayerAsync(osuId);
	public async Task<User?> GetOrCreateSystemUserAsync() => await _repository.GetOrCreateSystemUserAsync();
	public async Task<bool> HasRoleAsync(long osuId, string role) => await _repository.HasRoleAsync(osuId, role);
}