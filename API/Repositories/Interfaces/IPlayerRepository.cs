using API.Entities;

namespace API.Repositories.Interfaces;

public interface IPlayerRepository : IRepository<Player>
{
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
    Task<Player?> GetVersatileAsync(string key);

    /// <summary>
    /// Gets a list of players for the given username
    /// </summary>
    /// <remarks>Username is case insensitive and partially matched</remarks>
    Task<IEnumerable<Player>> SearchAsync(string username);

    /// <summary>
    /// Gets a player for the given username
    /// </summary>
    /// <remarks>Username is case insensitive and partially matched</remarks>
    /// <returns>A player, or null if not found</returns>
    Task<Player?> GetAsync(string username);

    /// <summary>
    /// Gets a player for the given osu! id
    /// </summary>
    /// <returns>A player, or null if not found</returns>
    Task<Player?> GetAsync(long osuId);

    /// <summary>
    /// Returns a collection of players, one per provided osu! id
    /// </summary>
    /// <param name="osuIds">The osu! player ids</param>
    /// <returns>One <see cref="Player"/> per osu! id match, null if no match found</returns>
    Task<IEnumerable<Player?>> GetAsync(IEnumerable<long> osuIds);

    /// <summary>
    /// Gets a player for the given osu! id, or creates one if one doesn't exist
    /// </summary>
    /// <returns>The found or created player</returns>
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
    /// Gets the username of a player for the given id
    /// </summary>
    /// <returns>A username, or null if not found</returns>
    Task<string?> GetUsernameAsync(int id);

    /// <summary>
    /// Gets the country of a player for the given id
    /// </summary>
    /// <returns>A country code, or null if not found</returns>
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
