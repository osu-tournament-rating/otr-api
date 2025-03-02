using Common.Enums.Enums;
using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IPlayerTournamentStatsRepository : IRepository<PlayerTournamentStats>
{
    /// <summary>
    /// Retrieves all tournament statistics for a specific player based on the provided criteria.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player.</param>
    /// <param name="ruleset">The ruleset to filter the tournament statistics.</param>
    /// <param name="dateMin">The minimum date to filter the tournament statistics. If null, no lower date limit is applied.</param>
    /// <param name="dateMax">The maximum date to filter the tournament statistics. If null, no upper date limit is applied.</param>
    /// <returns>A collection of tournament statistics for the specified player.</returns>
    Task<ICollection<PlayerTournamentStats>> GetForPlayerAsync(int playerId, Ruleset ruleset, DateTime? dateMin, DateTime? dateMax);

    /// <summary>
    /// Retrieves the best performances of a player in tournaments based on the provided criteria.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player.</param>
    /// <param name="count">The number of best performances to retrieve.</param>
    /// <param name="ruleset">The ruleset to filter the tournament statistics.</param>
    /// <param name="dateMin">The minimum date to filter the tournament statistics. If null, no lower date limit is applied.</param>
    /// <param name="dateMax">The maximum date to filter the tournament statistics. If null, no upper date limit is applied.</param>
    /// <returns>A collection of the best tournament performances for the specified player.</returns>
    Task<ICollection<PlayerTournamentStats>> GetBestPerformancesAsync(int playerId, int count, Ruleset ruleset, DateTime? dateMin, DateTime? dateMax);

    /// <summary>
    /// Retrieves the most recent performances of a player in tournaments based on the provided criteria.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player.</param>
    /// <param name="count">The number of recent performances to retrieve.</param>
    /// <param name="ruleset">The ruleset to filter the tournament statistics.</param>
    /// <param name="dateMin">The minimum date to filter the tournament statistics. If null, no lower date limit is applied.</param>
    /// <param name="dateMax">The maximum date to filter the tournament statistics. If null, no upper date limit is applied.</param>
    /// <returns>A collection of the most recent tournament performances for the specified player.</returns>
    Task<ICollection<PlayerTournamentStats>> GetRecentPerformancesAsync(int playerId, int count, Ruleset ruleset, DateTime? dateMin, DateTime? dateMax);
}
