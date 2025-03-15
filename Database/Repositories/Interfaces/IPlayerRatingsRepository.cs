using Common.Enums;
using Database.Entities.Processor;
using Database.Models;

namespace Database.Repositories.Interfaces;

public interface IPlayerRatingsRepository : IRepository<PlayerRating>
{
    /// <summary>
    ///     Get a rating for a given player and <see cref="Ruleset" />.
    /// </summary>
    /// <param name="playerId">Player id</param>
    /// <param name="ruleset">Ruleset</param>
    /// <returns>
    ///     A <see cref="PlayerRating" /> for the given playerId and <see cref="Ruleset" />,
    ///     or null if not found
    /// </returns>
    Task<PlayerRating?> GetAsync(int playerId, Ruleset ruleset, bool includeAdjustments = false);

    /// <summary>
    /// Get a list of rulesets for which the player has a rating
    /// </summary>
    /// <param name="playerId">Player id</param>
    /// <returns>All rulesets which the player has a rating for</returns>
    Task<IList<Ruleset>> GetActiveRulesetsAsync(int playerId);

    /// <summary>
    ///  The highest numeric (aka the worst) rank of a player in our system.
    /// </summary>
    /// <param name="country"></param>
    /// <returns></returns>
    Task<int> HighestRankAsync(Ruleset ruleset, string? country = null);

    // TODO: Remove - web should hardcode a value like 3,500 rating as the max for the slider.
    // TODO: that's all this method is used for.
    /// <summary>
    ///     The highest rating ever achieved for a given <see cref="Ruleset" /> and country
    /// </summary>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="country">Country code</param>
    /// <returns>Highest achieved rating for the <see cref="Ruleset" /> and country code</returns>
    Task<double> HighestRatingAsync(Ruleset ruleset, string? country = null);

    // TODO: Remove - web should hardcode a value like 500 matches as the max for the slider.
    // TODO: that's all this method is used for.
    Task<int> HighestMatchesAsync(Ruleset ruleset, string? country = null);

    /// <summary>
    /// Histogram of all ratings for a given <see cref="Ruleset"/>
    /// </summary>
    /// <param name="ruleset">Ruleset</param>
    /// <returns>
    ///  A dictionary with the keys equal to the 'bucket' of rating displayed (i.e. 100, 125, 150, etc. rating)
    ///  in the histogram, and the values being how many players have ratings within the buckets.
    /// </returns>
    Task<IDictionary<int, int>> GetHistogramAsync(Ruleset ruleset);

    /// <summary>
    /// Retrieves a paginated leaderboard of player ratings for a specific ruleset and chart type.
    /// </summary>
    /// <param name="page">The page number to retrieve (1-based index).</param>
    /// <param name="pageSize">The number of entries per page.</param>
    /// <param name="ruleset">The ruleset for which the leaderboard is being retrieved.</param>
    /// <param name="chartType">The type of chart (e.g. country or global).</param>
    /// <param name="filter">Optional filter to apply to the leaderboard (e.g. rank range).</param>
    /// <param name="country">Optional country code to filter by.</param>
    /// <returns>A collection of <see cref="PlayerRating"/> representing the leaderboard entries.</returns>
    /// <remarks>The chartType must be Country for the country parameter to be applied.
    Task<IEnumerable<PlayerRating>> GetLeaderboardAsync(
        int page,
        int pageSize,
        Ruleset ruleset,
        LeaderboardChartType chartType,
        LeaderboardFilter? filter,
        string? country
    );

    /// <summary>
    /// Retrieves the total count of players in the leaderboard for a specific ruleset and chart type,
    /// optionally filtered.
    /// </summary>
    /// <param name="requestQueryRuleset">The ruleset for which the leaderboard count is being retrieved.</param>
    /// <param name="chartType">The type of chart (e.g., country or global) to base the leaderboard on.</param>
    /// <param name="filter">Optional filter to apply to the leaderboard (e.g., rank range).</param>
    /// <param name="country">Optional country to filter by</param>
    /// <returns>The total number of players in the leaderboard matching the criteria.</returns>
    /// <remarks>The chartType must be Country for the country parameter to be applied.
    Task<int> LeaderboardCountAsync(
        Ruleset requestQueryRuleset,
        LeaderboardChartType chartType,
        LeaderboardFilter filter,
        string? country);
}
