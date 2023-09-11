using API.DTOs;
using API.Models;
using API.Osu;

namespace API.Services.Interfaces;

public interface IPlayerService : IService<Player>
{
	Task<PlayerDTO?> GetByOsuIdAsync(long osuId);
	Task<IEnumerable<PlayerDTO>> GetByOsuIdsAsync(IEnumerable<long> osuIds);
	Task<int> GetIdByOsuIdAsync(long osuId);
	Task<long> GetOsuIdByIdAsync(int id);
	Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync();
	Task<IEnumerable<Unmapped_PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode);
	/// <summary>
	/// Returns players that haven't been updated in the last 14 days,
	/// or players that have never been updated.
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<Player>> GetOutdatedAsync();

	Task<IEnumerable<PlayerDTO>> GetAllAsync();
}