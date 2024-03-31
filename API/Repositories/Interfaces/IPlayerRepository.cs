using API.Entities;

namespace API.Repositories.Interfaces;

public interface IPlayerRepository : IRepository<Player>
{
    /// <summary>
    /// Gets a list of players for the given username
    /// </summary>
    /// <remarks>Username is case insensitive</remarks>
    Task<IEnumerable<Player>> SearchAsync(string username);

    /// <summary>
    /// Gets a player for the given username
    /// </summary>
    /// <remarks>Username is case insensitive</remarks>
    /// <returns>A player, or null if not found</returns>
    Task<Player?> GetAsync(string username);

    /// <summary>
    /// Gets a player for the given osu! id
    /// </summary>
    /// <returns>A player, or null if not found</returns>
    Task<Player?> GetAsync(long osuId);

    /// <summary>
    /// Gets a player for the given osu! id, or creates one if one doesn't exist
    /// </summary>
    Task<Player> GetOrCreateAsync(long osuId);

    /// <summary>
    /// Gets the id of a player for the given username
    /// </summary>
    /// <remarks>Username is case insensitive</remarks>
    /// <returns>A player id, or null if not found</returns>
    Task<int?> GetIdAsync(string username);

    /// <summary>
    /// Gets the id of a player for the given osu! id
    /// </summary>
    /// <returns>A player id, or null if not found</returns>
    Task<int?> GetIdAsync(long osuId);

    /// <summary>
    /// Gets the osu! id for the given player id
    /// </summary>
    /// <returns>An osu! id, or null if not found</returns>
    Task<long> GetOsuIdAsync(int id);

    /// <summary>
    /// Fetches the username for the given player id
    /// </summary>
    /// <param name="id">The user id</param>
    Task<string?> GetUsernameAsync(int id);

    /// <summary>
    /// Returns the country of the player with the given player id, if available
    /// </summary>
    /// <param name="id"></param>
    Task<string?> GetCountryAsync(int id);

    /// <summary>
    /// Gets all players that haven't been updated in the last 14 days
    /// or have never been updated
    /// </summary>
    /// <remarks>This is used by a scheduled task to automatically populate user info, such as username, country, etc</remarks>
    Task<IEnumerable<Player>> GetOutdatedAsync();

    /// <summary>
    /// Gets all players that are missing an earliest global rank in any mode (but have a current rank in that mode)
    /// </summary>
    /// <remarks>This is used by a scheduled task to automatically populate user info, such as username, country, etc.</remarks>
    Task<IEnumerable<Player>> GetMissingRankAsync();
}
