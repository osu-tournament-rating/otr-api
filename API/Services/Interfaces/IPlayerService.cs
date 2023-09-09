using API.Models;

namespace API.Services.Interfaces;

public interface IPlayerService : IService<Player>
{
	Task<Player?> GetByOsuIdAsync(long osuId);
	Task<IEnumerable<Player>> GetByOsuIdsAsync(IEnumerable<long> osuIds);
	Task<int> GetIdByOsuIdAsync(long osuId);
	Task<long> GetOsuIdByIdAsync(int id);
	/// <summary>
	/// Returns players that haven't been updated in the last 14 days,
	/// or players that have never been updated.
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<Player>> GetOutdatedAsync();

	Task<IEnumerable<Player>> GetAllAsync();
}