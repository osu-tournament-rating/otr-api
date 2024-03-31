using API.DTOs;

namespace API.Services.Interfaces;

public interface IPlayerService
{
    /// <summary>
    /// Gets all players
    /// </summary>
    Task<IEnumerable<PlayerDTO>> GetAllAsync();

    /// <summary>
    /// Gets all player ranks
    /// </summary>
    Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync();

    /// <summary>
    /// Gets a player by dynamically searching for the given key
    /// </summary>
    /// <remarks>
    /// Searches via the following, in order of priority:
    /// - Player id
    /// - osu! id
    /// - osu! username (case insensitive)
    /// </remarks>
    /// <param name="key">The dynamic key of the player to look for</param>
    /// <returns>A player, or null if not found</returns>
    Task<PlayerDTO?> GetVersatileAsync(string key);

    /// <summary>
    /// Gets a player for the given id
    /// </summary>
    /// <returns>A player, or null if not found</returns>
    Task<PlayerDTO?> GetAsync(int id);

    /// <summary>
    /// Gets a player for the given osu! id
    /// </summary>
    /// <returns>A player, or null if not found</returns>
    Task<PlayerDTO?> GetAsync(long osuId);

    /// <summary>
    /// Gets a player for the given osu! username
    /// </summary>
    /// <returns>A player, or null if not found</returns>
    Task<PlayerDTO?> GetAsync(string username);

    /// <summary>
    /// Updates a player entity
    /// </summary>
    /// <remarks>Will not update Id field</remarks>
    /// <param name="id">Id of the target player</param>
    /// <param name="wrapper">New values for the target player</param>
    /// <returns>An updated player, or null if not found</returns>
    Task<PlayerDTO?> UpdateAsync(int id, PlayerDTO wrapper);
}
