using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IPlayersRepository : IRepository<Player>
{
    /// <summary>
    /// Gets a player for the given osu! id
    /// </summary>
    /// <param name="osuId">Player osu! id</param>
    /// <returns>A player, or null if not found</returns>
    Task<Player?> GetByOsuIdAsync(long osuId);

    /// <summary>
    /// Gets a player for each given osu! id
    /// </summary>
    /// <param name="osuIds">Player osu! ids</param>
    /// <returns>A list containing a player for each osu! id. If one is not found, it will not be returned.</returns>
    Task<IEnumerable<Player>> GetByOsuIdAsync(IEnumerable<long> osuIds);

    /// <summary>
    /// Returns a list of players that matches the given username. Case insensitive
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<IEnumerable<Player>> SearchAsync(string username);

    /// <summary>
    /// Gets a player filtering by the following, in order of priority: (id, osu! id, username)
    /// </summary>
    /// <remarks>Username filter uses strict matching, case insensitive</remarks>
    /// <param name="key">The dynamic key to filter for</param>
    /// <param name="eagerLoad">If true, includes the <see cref="User"/> and <see cref="UserSettings"/></param>
    /// <returns>A player, or null if not found</returns>
    Task<Player?> GetVersatileAsync(string key, bool eagerLoad);

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
    /// Returns a collection of players, one per provided osu! id
    /// </summary>
    /// <param name="osuIds">The osu! player ids</param>
    /// <returns>One <see cref="Player"/> per osu! id match, null if no match found</returns>
    Task<IEnumerable<Player?>> GetAsync(IEnumerable<long> osuIds);

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
    /// Returns the player id for the given user id
    /// </summary>
    /// <param name="userId"></param>
    Task<int> GetIdAsync(int userId);

    /// <summary>
    /// Returns the user id for the given player id, if available
    /// </summary>
    /// <param name="id">The player id</param>
    Task<int?> GetUserIdAsync(int id);

    /// <summary>
    /// Returns the country of the player with the given player id, if available
    /// </summary>
    /// <param name="playerId"></param>
    Task<string?> GetCountryAsync(int playerId);

    /// <summary>
    /// Sets all players' <see cref="Player.OsuLastFetch"/> to an outdated value based on the given timespan
    /// </summary>
    /// <param name="outdatedAfter">Timespan to check against the date of last access to the osu! API</param>
    Task SetOutdatedOsuAsync(TimeSpan outdatedAfter);

    /// <summary>
    /// Gets players with outdated osu! API data
    /// </summary>
    /// <param name="outdatedAfter">Timespan to check against the date of last access to the osu! API</param>
    /// <param name="limit">Maximum number of players</param>
    Task<IEnumerable<Player>> GetOutdatedOsuAsync(TimeSpan outdatedAfter, int limit);

    /// <summary>
    /// Sets all players' <see cref="Player.OsuTrackLastFetch"/> to an outdated value based on the given timespan
    /// </summary>
    /// <param name="outdatedAfter">Timespan to check against the date of last access to the osu!Track API</param>
    Task SetOutdatedOsuTrackAsync(TimeSpan outdatedAfter);

    /// <summary>
    /// Gets players with outdated osu!Track API data
    /// </summary>
    /// <param name="outdatedAfter">Timespan to check against the date of last access to the osu!Track API</param>
    /// <param name="limit">Maximum number of players</param>
    Task<IEnumerable<Player>> GetOutdatedOsuTrackAsync(TimeSpan outdatedAfter, int limit);
}
