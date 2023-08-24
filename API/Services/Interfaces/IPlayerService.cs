using API.Entities;

namespace API.Services.Interfaces;

public interface IPlayerService : IService<Player>
{
	Task<Player?> GetByOsuIdAsync(int osuId);
	Task<int> GetIdByOsuIdAsync(long osuId);
	Task<long> GetOsuIdByIdAsync(int id);
}