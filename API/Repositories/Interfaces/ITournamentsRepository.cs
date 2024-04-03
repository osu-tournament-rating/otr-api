using API.DTOs;
using API.Entities;

namespace API.Repositories.Interfaces;

public interface ITournamentsRepository : IRepository<Tournament>
{
    /// <summary>
    /// Get a <see cref="Tournament"/> entity
    /// </summary>
    /// <param name="id">Primary key</param>
    /// <param name="eagerLoad">Whether to eagerly load navigational properties</param>
    Task<Tournament?> GetAsync(int id, bool eagerLoad = false);

    /// <summary>
    /// Returns whether an entity with the given name and mode exists
    /// </summary>
    public Task<bool> ExistsAsync(string name, int mode);

    /// <summary>
    /// Search for a tournament by name
    /// </summary>
    public Task<IEnumerable<Tournament>> SearchAsync(string name);

    /// <summary>
    /// Create team size statistics for a player
    /// </summary>
    /// <param name="playerId">Id of target player</param>
    /// <param name="mode">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    public Task<PlayerTournamentTeamSizeCountDTO> GetTeamSizeStatsAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    );

    /// <summary>
    /// Returns a list of best or worst tournament performances for a player
    /// </summary>
    /// <param name="playerId">Id (primary key) of target player</param>
    /// <param name="mode">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    /// <param name="count">Size of results</param>
    /// <param name="bestPerformances">Sort by best or worst performance</param>
    Task<IEnumerable<PlayerTournamentMatchCostDTO>> GetPerformancesAsync(int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax,
        int count,
        bool bestPerformances);

    /// <summary>
    /// Count number of tournaments played for a player
    /// </summary>
    /// <param name="playerId">Id of target player</param>
    /// <param name="mode">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    Task<int> CountPlayedAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);
}
