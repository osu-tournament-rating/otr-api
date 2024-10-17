using Database.Entities.Processor;
using Database.Enums;

namespace Database.Repositories.Interfaces;

public interface IPlayerRatingRepository : IRepository<PlayerRating>
{
    /// <summary>
    ///  Fetch all <see cref="PlayerRating"/> data for a <see cref="Player"/>
    /// </summary>
    /// <param name="id">The player id</param>
    /// <returns>A collection of <see cref="PlayerRating"/> for the <see cref="Player"/> - one per ruleset</returns>
    Task<IEnumerable<PlayerRating>> GetForPlayerAsync(int id);

    /// <summary>
    /// Fetch a <see cref="PlayerRating"/> for a given <see cref="Player"/> and <see cref="Ruleset"/>
    /// </summary>
    /// <param name="playerId">The player id</param>
    /// <param name="ruleset">The ruleset</param>
    /// <returns>A <see cref="PlayerRating"/> for the player and ruleset or null if not found</returns>
    Task<PlayerRating?> GetForPlayerAsync(int playerId, Ruleset ruleset);

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
