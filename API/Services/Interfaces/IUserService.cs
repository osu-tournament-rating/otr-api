using API.Models;

namespace API.Services.Interfaces;

public interface IUserService : IService<User>
{
	Task<User> GetForPlayerAsync(int playerId);
}