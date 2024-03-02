using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IUserService
{
	// TODO: Change to DTOs
	/// <summary>
	/// Returns a UserInfoDTO if the user exists
	/// </summary>
	/// <param name="id">The id of the user</param>
	/// <returns></returns>
	Task<UserInfoDTO?> GetAsync(int id);
	Task<UserInfoDTO?> GetForPlayerAsync(int playerId);
	Task<User?> GetForPlayerAsync(long osuId);
	Task<User?> GetOrCreateSystemUserAsync();
	Task<bool> HasRoleAsync(long osuId, string role);
}