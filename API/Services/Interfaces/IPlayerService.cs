using API.DTOs;
using API.Osu;

namespace API.Services.Interfaces;

public interface IPlayerService
{
    Task<IEnumerable<PlayerDTO>> GetAllAsync();
    Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync();
    Task<IEnumerable<PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode);
    Task<string?> GetUsernameAsync(long osuId);
    Task<int?> GetIdAsync(long osuId);
    Task<int?> GetIdAsync(int userId);
    Task<long?> GetOsuIdAsync(int id);
    Task<PlayerDTO?> GetByUsernameAsync(string username);

    /// <summary>
    /// A unique mapping of osu! user ids to our internal ids.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync();

    Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync();
    /// <summary>
    /// Dynamically searches for players via the following, in order of priority:
    ///
    /// If the key can be parsed as an integer:
    /// - The player id
    /// - The osu id
    ///
    /// Searches for the username (case insensitive) otherwise.
    ///
    /// Returns null if no matches found.
    /// </summary>
    /// <param name="key">The dynamic key of the player to look for</param>
    /// <returns></returns>
    Task<PlayerInfoDTO?> GetVersatileAsync(string key);
    Task<PlayerInfoDTO?> GetAsync(int userId);
    Task<PlayerInfoDTO?> GetAsync(long osuId);
    Task<PlayerInfoDTO?> GetAsync(string username);
}
