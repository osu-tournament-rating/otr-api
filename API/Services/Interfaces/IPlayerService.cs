using API.DTOs;
using API.Osu;

namespace API.Services.Interfaces;

public interface IPlayerService
{
	Task<IEnumerable<PlayerDTO>> GetAllAsync();
	Task<PlayerDTO?> GetByOsuIdAsync(long osuId, bool eagerLoad = false, OsuEnums.Mode mode = OsuEnums.Mode.Standard, int offsetDays = -1);
	Task<IEnumerable<PlayerDTO>> GetByOsuIdsAsync(IEnumerable<long> osuIds);
	Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync();
	Task<IEnumerable<PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode);
	Task<string?> GetUsernameAsync(long osuId);
	Task<int?> GetIdAsync(long osuId);
	Task<long?> GetOsuIdAsync(int id);
	/// <summary>
	/// A unique mapping of osu! user ids to our internal ids.
	/// </summary>
	/// <returns></returns>
	Task<Dictionary<long, int>> GetIdMappingAsync();

	Task<Dictionary<int, string?>> GetCountryMappingAsync();
}