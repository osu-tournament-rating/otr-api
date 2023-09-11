using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IUserService : IService<User>
{
	Task<UserDTO?> GetForPlayerAsync(int playerId);
}