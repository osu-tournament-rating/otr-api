using Common.Enums;
using Database.Entities.Processor;

namespace Database.Repositories.Interfaces;

public interface IRatingAdjustmentsRepository : IRepository<RatingAdjustment>
{
    /// <summary>
    ///  Returns one entry per match, with each entry being all matches that occur on the same day.
    ///  Typically, each list will contain one item. However, if a player plays multiple matches in a day,
    ///  there will be multiple items in the list.
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="ruleset"></param>
    /// <param name="dateMin"></param>
    /// <param name="dateMax"></param>
    /// <returns></returns>
    Task<IEnumerable<RatingAdjustment>> GetForPlayerAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );

    /// <summary>
    /// Get peak ratings for multiple players in a single optimized query
    /// </summary>
    /// <param name="playerIds">Player IDs to fetch peak ratings for</param>
    /// <param name="ruleset">Ruleset filter</param>
    /// <param name="dateMin">Minimum date filter</param>
    /// <param name="dateMax">Maximum date filter</param>
    /// <returns>Dictionary mapping player IDs to their peak ratings</returns>
    Task<Dictionary<int, double?>> GetPeakRatingsForPlayersAsync(
        IEnumerable<int> playerIds,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );

    /// <summary>
    /// Get all rating adjustments for a specific match
    /// </summary>
    /// <param name="matchId">The match ID to get adjustments for</param>
    /// <returns>Collection of rating adjustments for the match</returns>
    Task<IEnumerable<RatingAdjustment>> GetForMatchAsync(int matchId);
}
