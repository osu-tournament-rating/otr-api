using API.DTOs;
using API.Entities;
using API.Osu;

namespace API.Repositories.Interfaces;

public interface IPlayerRepository : IRepository<Player>
{
	Task<Player?> GetAsync(string username);
	Task<Player?> GetPlayerByOsuIdAsync(long osuId, bool eagerLoad = false, int mode = 0, int offsetDays = -1);
	Task<int> GetIdByOsuIdAsync(long osuId);
	Task<long> GetOsuIdAsync(int id);

	/// <summary>
	///  Returns players that haven't been updated in the last 14 days,
	///  or players that have never been updated.
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<Player>> GetOutdatedAsync();

	Task<IEnumerable<Player>> GetPlayersWhereMissingGlobalRankAsync();
	Task<IEnumerable<Player>> GetByOsuIdsAsync(IEnumerable<long> osuIds);
	Task<IEnumerable<Player>> GetAllAsync(bool eagerLoad = false);
	Task<IEnumerable<PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode);
	Task<string?> GetUsernameAsync(long? osuId);
	Task<string?> GetUsernameAsync(int? id);
	Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync();
	Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync();
	Task<int> GetIdByUserIdAsync(int userId);
	Task<string?> GetCountryAsync(int playerId);
	Task<int> GetIdAsync(string username);
}