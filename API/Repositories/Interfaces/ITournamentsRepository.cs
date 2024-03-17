using API.DTOs;
using API.Entities;

namespace API.Repositories.Interfaces;

public interface ITournamentsRepository : IRepository<Tournament>
{
    /// <summary>
    /// Get a <see cref="Tournament"/> entity
    /// </summary>
    /// <param name="id">Primary key</param>
    /// <param name="eagerLoad">Whether to include navigational properties</param>
    /// <returns></returns>
    Task<Tournament?> GetAsync(int id, bool eagerLoad = false);

    Task<int> CountPlayedAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null);

    public Task<bool> ExistsAsync(string name, int mode);

    public Task<PlayerTournamentTeamSizeCountDTO> GetPlayerTeamSizeStatsAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    );

    /// <summary>
    ///  Finds and returns the best or worst tournaments for a player, rated and ordered by average match cost.
    /// </summary>
    /// <param name="count">The number of tournaments to return</param>
    /// <returns>A list of <see cref="count" /> tournaments ordered by the player's average match cost, descending</returns>
    Task<IEnumerable<PlayerTournamentMatchCostDTO>> GetPerformancesAsync(
        int count,
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax,
        bool bestPerformances
    );
}
