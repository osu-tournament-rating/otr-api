using API.Entities;

namespace API.Services.Interfaces;

public interface IUserService : IService<User>
{
	Task<User> GetByPlayerIdAsync(int playerId);
}