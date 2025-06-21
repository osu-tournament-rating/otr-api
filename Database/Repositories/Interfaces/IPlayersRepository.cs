using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IPlayersRepository : IRepository<Player>
{
    /// <summary>
    /// Returns a list of players that matches the given username. Case insensitive
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<IEnumerable<Player>> SearchAsync(string username);

    /// <summary>
    /// Gets a player filtering by the following, in order of priority: (id, osu! id, username)
    /// </summary>
    /// <remarks>Username filter uses strict matching, case-insensitive</remarks>
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
    /// Fetch multiple <see cref="Player"/>s by primary key
    /// </summary>
    /// <param name="ids">Primary keys</param>
    /// <returns>
    /// A collection of <see cref="Player"/>s, one per provided id, if it exists
    /// </returns>
    Task<ICollection<Player>> GetAsync(IEnumerable<int> ids);

    /// <summary>
    /// Returns a collection of players, one per provided osu! id
    /// </summary>
    /// <param name="osuIds">The osu! player ids</param>
    /// <returns>One <see cref="Player"/> per osu! id match, null if no match found</returns>
    Task<IEnumerable<Player>> GetAsync(IEnumerable<long> osuIds);

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
