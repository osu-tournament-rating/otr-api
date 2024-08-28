using API.DTOs;
using API.Enums;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IApiTournamentsRepository : ITournamentsRepository
{
    /// <summary>
    /// Search for a tournament by name
    /// </summary>
    public Task<IEnumerable<TournamentSearchResultDTO>> SearchAsync(string name);

    /// <summary>
    /// Create team size statistics for a player
    /// </summary>
    /// <param name="playerId">Id of target player</param>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    public Task<PlayerTournamentLobbySizeCountDTO> GetLobbySizeStatsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    );

    /// <summary>
    /// Returns a list of best or worst tournament performances for a player
    /// </summary>
    /// <param name="playerId">Id (primary key) of target player</param>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="dateMin">Date lower bound</param>
    /// <param name="dateMax">Date upper bound</param>
    /// <param name="count">Size of results</param>
    /// <param name="bestPerformances">Sort by best or worst performance</param>
    Task<IEnumerable<PlayerTournamentMatchCostDTO>> GetPerformancesAsync(int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax,
        int count,
        TournamentPerformanceResultType performanceType
    );
}
