using API.DTOs;

namespace API.Services.Interfaces;

public interface IPlayerService
{
    Task<IEnumerable<PlayerCompactDTO>> GetAllAsync();

    Task<int?> GetIdAsync(int userId);

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
    Task<PlayerCompactDTO?> GetVersatileAsync(string key);

    /// <summary>
    /// Gets player information for a list of osu! ids
    /// </summary>
    /// <param name="osuIds">The osu! player ids</param>
    /// <returns>A list of <see cref="PlayerCompactDTO"/>, one per provided osu! id.
    /// If a provided osu! id does not belong to a player in the database,
    /// the <see cref="PlayerCompactDTO"/> will be returned in a default state,
    /// except the <see cref="PlayerCompactDTO.OsuId"/> value will be set</returns>
    Task<IEnumerable<PlayerCompactDTO>> GetAsync(IEnumerable<long> osuIds);

    /// <summary>
    /// Checks if the player exists
    /// </summary>
    /// <param name="id">The player id</param>
    /// <returns>True if the player exists, false otherwise</returns>
    Task<bool> ExistsAsync(int id);
}
