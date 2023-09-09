using API.DTOs;
using API.Models;

namespace API.Services.Interfaces;

public interface IUserService : IService<User>
{
	Task<UserDTO?> GetForPlayerAsync(int playerId);
}