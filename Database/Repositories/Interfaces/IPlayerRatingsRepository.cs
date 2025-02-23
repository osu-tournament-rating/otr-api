using Database.Entities.Processor;
using Database.Enums;

namespace Database.Repositories.Interfaces;

public interface IPlayerRatingsRepository : IRepository<PlayerRating>
{
    /// <summary>
    /// Get all ratings for a player by osu! id, one for each <see cref="Ruleset"/>> (if available)
    /// </summary>
    /// <param name="playerId">The osu! player id</param>
    /// <returns>A collection of <see cref="PlayerRating"/>s, one per ruleset</returns>
    Task<IEnumerable<PlayerRating>> GetAsync(long osuPlayerId);

    /// <summary>
    ///     Get a rating for a given player and <see cref="Ruleset" />.
    /// </summary>
    /// <param name="playerId">Player id</param>
    /// <param name="ruleset">Ruleset</param>
    /// <returns>
    ///     A <see cref="PlayerRating" /> for the given playerId and <see cref="Ruleset" />,
    ///     or null if not found
    /// </returns>
    Task<PlayerRating?> GetAsync(int playerId, Ruleset ruleset);

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
}
