using API.DTOs;

namespace API.Services.Interfaces;

public interface IPlayerService
{
    /// <summary>
    /// Searches for a player using the provided key.
    /// Priority is given to numerical keys, attempting to interpret them as either a player ID or an osu! ID (in that order).
    /// If the key is not a number, a case-insensitive username search is performed.
    /// </summary>
    /// <param name="key">The search key for identifying the player</param>
    /// <returns>Returns null if no matching player is found</returns>
    Task<PlayerCompactDTO?> GetVersatileAsync(string key);

    /// <summary>
    /// Retrieves player information for a collection of osu! IDs
    /// </summary>
    /// <param name="osuIds">A collection of osu! player IDs to look up</param>
    /// <returns>
    /// A collection of <see cref="PlayerCompactDTO"/> objects matching the order of the provided osu! IDs.
    /// If a player with a specific osu! ID is not found in the database, null is returned at that position.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="osuIds"/> is null</exception>
    Task<IEnumerable<PlayerCompactDTO?>> GetAsync(IEnumerable<long> osuIds);
}
