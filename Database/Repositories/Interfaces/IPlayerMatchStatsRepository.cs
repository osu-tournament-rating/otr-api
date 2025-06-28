using Common.Enums;
using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IPlayerMatchStatsRepository
{
    /// <summary>
    ///  A list of all matches played by a player in a given ruleset between two dates. Ordered by match start time.
    /// </summary>
    /// <param name="playerId">Player id</param>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="dateMin">Minimum inclusion date</param>
    /// <param name="dateMax">Maximum inclusion date</param>
    /// <returns></returns>
    Task<IEnumerable<PlayerMatchStats>> GetForPlayerAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin,
        DateTime? dateMax
    );

    Task<int> CountMatchesPlayedAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );

    /// <summary>
    /// Count matches played for multiple players in a single query
    /// </summary>
    /// <param name="playerIds">Player ids</param>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="dateMin">Minimum inclusion date</param>
    /// <param name="dateMax">Maximum inclusion date</param>
    /// <returns>Dictionary mapping player IDs to their match count</returns>
    Task<Dictionary<int, int>> CountMatchesPlayedAsync(
        IEnumerable<int> playerIds,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );

    Task<double> GlobalWinrateAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );

    /// <summary>
    /// Get global win rates for multiple players in a single query
    /// </summary>
    /// <param name="playerIds">Player ids</param>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="dateMin">Minimum inclusion date</param>
    /// <param name="dateMax">Maximum inclusion date</param>
    /// <returns>Dictionary mapping player IDs to their win rate</returns>
    Task<Dictionary<int, double>> GlobalWinrateAsync(
        IEnumerable<int> playerIds,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );
}
