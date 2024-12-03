using Database.Entities.Processor;
using Database.Enums;

namespace Database.Repositories.Interfaces;

public interface IPlayerRatingsRepository : IRepository<PlayerRating>
{
    /// <summary>
    /// Returns all ratings for a player, one for each <see cref="Ruleset"/>> (if available)
    /// </summary>
    /// <param name="playerId">The osu! player id</param>
    /// <returns>A collection of <see cref="PlayerRating"/>, one per ruleset</returns>
    Task<IEnumerable<PlayerRating>> GetForPlayerAsync(long osuPlayerId);

    Task<PlayerRating?> GetForPlayerAsync(int playerId, Ruleset ruleset);


    Task<int> BatchInsertAsync(IEnumerable<PlayerRating> playerRatings);

    /// <summary>
    ///  The highest numeric (aka the worst) rank of a player in our system.
    /// </summary>
    /// <param name="country"></param>
    /// <returns></returns>
    Task<int> HighestRankAsync(Ruleset ruleset, string? country = null);

    Task<double> HighestRatingAsync(Ruleset ruleset, string? country = null);

    Task<int> HighestMatchesAsync(Ruleset ruleset, string? country = null);

    /// <summary>
    /// </summary>
    /// <param name="ruleset"></param>
    /// <returns>
    ///  A dictionary with the keys equal to the 'bucket' of rating displayed
    ///  in the histogram, and the values being how many players have ratings within the buckets.
    /// </returns>
    Task<IDictionary<int, int>> GetHistogramAsync(Ruleset ruleset);
}
