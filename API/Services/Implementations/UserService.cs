using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class UserService : IUserService
{
	private readonly IUserRepository _repository;

	public UserService(IUserRepository repository) { _repository = repository; }

	public async Task<MeDataDTO?> GetForPlayerAsync(int playerId)
	{
		var user = await _repository.GetForPlayerAsync(playerId);
		return new MeDataDTO
		{
			Id = user.PlayerId,
			OsuCountry = user.Player.Country,
			OsuId = user.Player.OsuId,
			OsuPlayMode = 0 // TODO: Set to user's preferred mode
		};
	}
	public async Task<User?> GetForPlayerAsync(long osuId) => await _repository.GetForPlayerAsync(osuId);
	public async Task<User?> GetOrCreateSystemUserAsync() => await _repository.GetOrCreateSystemUserAsync();
	public async Task<bool> HasRoleAsync(long osuId, string role) => await _repository.HasRoleAsync(osuId, role);
}