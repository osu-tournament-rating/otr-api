using API.DTOs;
using API.Entities;

namespace API.Repositories.Interfaces;

public interface IPlayerRepository : IRepository<Player>
{
    /// <summary>
    /// Gets a list of players for the given username
    /// </summary>
    /// <remarks>Search uses partial matching, case insensitive</remarks>
    /// <returns>Maximum 30 records</returns>
    Task<IEnumerable<Player>> SearchAsync(string username);

    /// <summary>
    /// Gets a player for the given username
    /// </summary>
    /// <remarks>Search uses strict matching, case insensitive</remarks>
    /// <returns>A player, or null if not found</returns>
    Task<Player?> GetAsync(string username);

    /// <summary>
    /// Gets a player for the given osu id, or creates one if one does not exist
    /// </summary>
    Task<Player> GetOrCreateAsync(long osuId);

    /// <summary>
    /// Gets all players
    /// </summary>
    /// <param name="eagerLoad">Whether to also load related fields (i.e. player matches)</param>
    Task<IEnumerable<Player>> GetAsync(bool eagerLoad = false);

    /// <summary>
    /// Gets a list of players for the given osu ids
    /// </summary>
    Task<IEnumerable<Player?>> GetAsync(IEnumerable<long> osuIds);

    /// <summary>
    /// Gets a player for the given osu id
    /// </summary>
    /// <returns>A player, or null if not found</returns>
    Task<Player?> GetByOsuIdAsync(long osuId);

    /// <summary>
    /// Gets the id of a player for the given osu id
    /// </summary>
    Task<int?> GetIdAsync(long osuId);

    /// <summary>
    /// Gets the id of a player for the given username
    /// </summary>
    /// <remarks>Search uses strict matching, case insensitive</remarks>
    Task<int?> GetIdAsync(string username);

    /// <summary>
    /// Gets the osu id of a player for the given id
    /// </summary>
    Task<long> GetOsuIdAsync(int id);

    /// <summary>
    /// Returns players that haven't been updated in the last 14 days,
    /// or players that have never been updated.
    /// </summary>
    Task<IEnumerable<Player>> GetOutdatedAsync();

    /// <summary>
    /// Get all players that are missing an earliest global rank in any mode (but have a current rank in that mode)
    /// </summary>
    Task<IEnumerable<Player>> GetPlayersMissingRankAsync();

    /// <summary>
    /// Gets the username of a player for the given osu id
    /// </summary>
    Task<string?> GetUsernameAsync(long osuId);

    /// <summary>
    /// Gets the username of a player for the given id
    /// </summary>
    Task<string?> GetUsernameAsync(int id);

    /// <summary>
    /// Returns a collection of <see cref="PlayerIdMappingDTO"/> for all players
    /// </summary>
    Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync();

    /// <summary>
    /// Returns a collection of <see cref="PlayerCountryMappingDTO"/> for all players
    /// </summary>
    Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync();

    /// <summary>
    /// Gets the id of a player for the given user id
    /// </summary>
    Task<int> GetIdAsync(int userId);

    /// <summary>
    /// Gets the country of a player for the given player id
    /// </summary>
    Task<string?> GetCountryAsync(int playerId);
}
