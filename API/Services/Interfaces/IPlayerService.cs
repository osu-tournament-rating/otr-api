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

    /// <summary>
    /// A unique mapping of osu! user ids to our internal ids.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync();

    Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync();
    Task<PlayerInfoDTO?> GetAsync(int userId);
    Task<PlayerInfoDTO?> GetAsync(string username);
}
