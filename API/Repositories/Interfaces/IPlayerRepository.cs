using API.DTOs;
using API.Entities;
using API.Osu;

namespace API.Repositories.Interfaces;

public interface IPlayerRepository : IRepository<Player>
{
	Task<Player?> GetAsync(string username);
	Task<Player?> GetAsync(long osuId, bool eagerLoad = false, int mode = 0, int offsetDays = -1);
	/// <summary>
	/// Returns players that match the provided enumerable of osuIds
	/// </summary>
	/// <param name="osuIds">A list of ids by which players should be fetched from</param>
	/// <returns></returns>
	Task<IEnumerable<Player>> GetAsync(IEnumerable<long> osuIds);
	/// <summary>
	/// Returns all players
	/// </summary>
	/// <param name="eagerLoad">Whether to also load related fields (i.e. player matches)</param>
	/// <returns></returns>
	Task<IEnumerable<Player>> GetAsync(bool eagerLoad = false);
	Task<int> GetIdAsync(long osuId);
	Task<long> GetOsuIdAsync(int id);

	/// <summary>
	///  Returns players that haven't been updated in the last 14 days,
	///  or players that have never been updated.
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<Player>> GetOutdatedAsync();
	/// <summary>
	/// Get all players that are missing an earliest global rank in any mode (but have a current rank in that mode)
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<Player>> GetPlayersMissingRankAsync();
	Task<IEnumerable<PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode);
	Task<string?> GetUsernameAsync(long? osuId);
	Task<string?> GetUsernameAsync(int? id);
	Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync();
	Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync();
	Task<int> GetIdByUserIdAsync(int userId);
	Task<string?> GetCountryAsync(int playerId);
	Task<int> GetIdAsync(string username);
}