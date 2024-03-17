using API.DTOs;
using API.Entities;
using API.Osu;

namespace API.Repositories.Interfaces;

public interface IPlayerRepository : IRepository<Player>
{
    /// <summary>
    /// Returns a player, if available, that matches the given username. Case insensitive
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<IEnumerable<Player>> SearchAsync(string username);

    /// <summary>
    /// Attempts to fetch a player by the osu id. If none exists, create a new player
    /// and return it.
    /// </summary>
    /// <param name="osuId">The osu id of the player</param>
    /// <returns></returns>
    Task<Player> GetOrCreateAsync(long osuId);

    /// <summary>
    /// Returns all players
    /// </summary>
    /// <param name="eagerLoad">Whether to also load related fields (i.e. player matches)</param>
    /// <returns></returns>
    Task<IEnumerable<Player>> GetAsync(bool eagerLoad = false);

    /// <summary>
    /// Returns the id of the player that has this osuId
    /// </summary>
    /// <param name="osuId"></param>
    /// <returns></returns>
    Task<int?> GetIdAsync(long osuId);

    /// <summary>
    /// Return the id belonging to the player with this username. Case insensitive,
    /// underscores and spaces cannot coexist with each other, so they are treated as unique.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<int?> GetIdAsync(string username);

    /// <summary>
    /// Returns the osu id that belongs to this player id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<long> GetOsuIdAsync(int id);

    /// <summary>
    ///  Returns players that haven't been updated in the last 14 days,
    ///  or players that have never been updated.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Player>> GetOutdatedAsync();

    /// <summary>
    /// Get all players that are missing an earliest global rank in any mode (but have a current rank in that mode)
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Player>> GetPlayersMissingRankAsync();

    /// <summary>
    /// Returns a collection of <see cref="PlayerRatingDTO"/> in order by rating descending
    /// for the given mode.
    /// </summary>
    /// <param name="n">The number of items to return</param>
    /// <param name="mode">The mode to get the ratings from</param>
    Task<IEnumerable<PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode);

    /// <summary>
    /// Fetches the username for the given osu id, if available
    /// </summary>
    /// <param name="osuId">The osu id</param>
    /// <returns></returns>
    Task<string?> GetUsernameAsync(long osuId);

    /// <summary>
    /// Fetches the username for the given player id
    /// </summary>
    /// <param name="id">The user id</param>
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
    /// Returns the player id for the given user id
    /// </summary>
    /// <param name="userId"></param>
    Task<int> GetIdAsync(int userId);

    /// <summary>
    /// Returns the country of the player with the given player id, if available
    /// </summary>
    /// <param name="playerId"></param>
    Task<string?> GetCountryAsync(int playerId);
}
