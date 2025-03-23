using Common.Enums;
using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IPlayerTournamentStatsRepository : IRepository<PlayerTournamentStats>
{
    /// <summary>
    /// Retrieves all tournament statistics for all player ids provided for the given ruleset
    /// </summary>
    /// <param name="playerIds">The ids of the players to fetch stats for</param>
    /// <param name="ruleset">The ruleset to filter the tournament statistics by</param>
    /// <returns>A Dictionary of <see cref="PlayerTournamentStats"/> with the key equal to the player's id</returns>
    Task<IDictionary<int, IList<PlayerTournamentStats>>> GetAsync(IEnumerable<int> playerIds, Ruleset ruleset);

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
