using API.DTOs;

namespace API.Services.Interfaces;

public interface IPlayerService
{
    /// <summary>
    /// Denotes whether a player exists for the given id
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Gets all players
    /// </summary>
    Task<IEnumerable<PlayerDTO>> GetAllAsync();

    /// <summary>
    /// Gets ranking information for all players
    /// </summary>
    Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync();

    /// <summary>
    /// Get the id of a player for the given user id
    /// </summary>
    Task<int?> GetIdAsync(int userId);

    /// <summary>
    /// Gets mappings of all player ids to their osu! ids
    /// </summary>
    Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync();

    /// <summary>
    /// Gets mappings of all player ids to their country codes
    /// </summary>
    Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync();

    /// <summary>
    /// Dynamically searches for players via the following, in order of priority:
    ///
    /// If the key can be parsed as an integer:
    /// - The player id
    /// - The osu id
    ///
    /// Searches for the username (case insensitive) otherwise.
    /// </summary>
    /// <param name="key">The dynamic key of the player to look for</param>
    /// <returns>A player, or null if not found</returns>
    Task<PlayerInfoDTO?> GetVersatileAsync(string key);

    /// <summary>
    /// Gets player information for the given user id
    /// </summary>
    /// <returns>A player, or null if not found</returns>
    Task<PlayerInfoDTO?> GetAsync(int userId);

    /// <summary>
    /// Gets player information for the given osu id
    /// </summary>
    /// <returns>A player, or null if not found</returns>
    Task<PlayerInfoDTO?> GetByOsuIdAsync(long osuId);

    /// <summary>
    /// Gets player information for the given username
    /// </summary>
    /// <returns>A player, or null if not found</returns>
    Task<PlayerInfoDTO?> GetAsync(string username);

    /// <summary>
    /// Gets player information for a list of osu! ids
    /// </summary>
    /// <param name="osuIds">The osu! player ids</param>
    /// <returns>
    /// A list of <see cref="PlayerInfoDTO"/>, one per provided osu! id.
    /// If a provided osu! id does not belong to a player in the database,
    /// the <see cref="PlayerInfoDTO"/> will be returned in a default state,
    /// except the <see cref="PlayerInfoDTO.OsuId"/> value will be set
    /// </returns>
    Task<IEnumerable<PlayerInfoDTO>> GetAsync(IEnumerable<long> osuIds);
}
