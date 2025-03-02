using Common.Enums.Enums;
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

    /// <summary>
    ///
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="ruleset"></param>
    /// <param name="dateMin"></param>
    /// <param name="dateMax"></param>
    /// <returns></returns>
    Task<IEnumerable<int>> GetTeammateIdsAsync(int playerId,
        Ruleset ruleset,
        DateTime? dateMin,
        DateTime? dateMax);

    Task<IEnumerable<int>> GetOpponentIdsAsync(int playerId, Ruleset ruleset, DateTime? dateMin, DateTime? dateMax);

    Task<IEnumerable<PlayerMatchStats>> TeammateStatsAsync(
        int playerId,
        int teammateId,
        Ruleset ruleset,
        DateTime? dateMin,
        DateTime? dateMax
    );

    Task<IEnumerable<PlayerMatchStats>> OpponentStatsAsync(
        int playerId,
        int opponentId,
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
    Task<int> CountMatchesWonAsync(
        int playerId,
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

    Task<Dictionary<int, double>> GetMatchCostsAsync(int playerId,
        Ruleset ruleset, DateTime? dateMin = null,
        DateTime? dateMax = null);
}
