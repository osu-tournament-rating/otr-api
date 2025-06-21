using Database.Entities;
using DataWorkerService.Configurations;
using DataWorkerService.Services.Implementations;

namespace DataWorkerService.Services.Interfaces;

/// <summary>
/// Interfaces the <see cref="PlayersService"/>
/// </summary>
public interface IPlayersService
{
    /// <summary>
    /// Sets all players' <see cref="Player.OsuLastFetch"/> to an outdated value based on the given configuration
    /// </summary>
    /// <param name="config">Configuration to control the manner in which the osu! API is fetched</param>
    Task SetAllOutdatedOsuApiAsync(PlayerFetchPlatformConfiguration config);

    /// <summary>
    /// Updates outdated <see cref="Player"/> osu! API data based on the given configuration
    /// </summary>
    /// <param name="config">Configuration to control the manner in which the osu! API is fetched</param>
    Task UpdateOutdatedFromOsuApiAsync(PlayerFetchPlatformConfiguration config);

    /// <summary>
    /// Sets all players' <see cref="Player.OsuTrackLastFetch"/> to an outdated value based on the given configuration
    /// </summary>
    /// <param name="config">Configuration to control the manner in which the osu!Track API is fetched</param>
    Task SetAllOutdatedOsuTrackApiAsync(PlayerFetchPlatformConfiguration config);

    /// <summary>
    /// Updates outdated <see cref="Player"/> osu!Track API data based on the given configuration
    /// </summary>
    /// <param name="config">Configuration to control the manner in which the osu!Track API is fetched</param>
    Task UpdateOutdatedFromOsuTrackApiAsync(PlayerFetchPlatformConfiguration config);
}
