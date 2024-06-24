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
    Task SetAllOutdatedOsuApiAsync(PlayerPlatformConfiguration config);

    /// <summary>
    /// Updates outdated <see cref="Player"/> osu! API data based on the given configuration
    /// </summary>
    /// <param name="config">Configuration to control the manner in which the osu! API is fetched</param>
    Task UpdateOutdatedFromOsuApiAsync(PlayerPlatformConfiguration config);

    /// <summary>
    /// Updates a single <see cref="Player"/> with data from the osu! API
    /// </summary>
    /// <param name="player">The player to update</param>
    /// <remarks>Does not save changes to the database</remarks>
    Task UpdateFromOsuApiAsync(Player player);
}
