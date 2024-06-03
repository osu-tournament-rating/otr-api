using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IBaseStatsRepository : IRepository<BaseStats>
{
    /// <summary>
    ///  Returns all ratings for a player, one for each game mode (if available)
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<IEnumerable<BaseStats>> GetForPlayerAsync(long osuPlayerId);


    Task<BaseStats?> GetForPlayerAsync(int playerId, int mode);


    Task<int> InsertOrUpdateForPlayerAsync(int playerId, BaseStats baseStats);


    Task<int> BatchInsertAsync(IEnumerable<BaseStats> baseStats);


    Task TruncateAsync();


    Task<int> GetGlobalRankAsync(long osuPlayerId, int mode);


    /// <summary>
    ///  The highest numeric (aka the worst) rank of a player in our system.
    /// </summary>
    /// <param name="country"></param>
    /// <returns></returns>
    Task<int> HighestRankAsync(int mode, string? country = null);


    Task<double> HighestRatingAsync(int mode, string? country = null);


    Task<int> HighestMatchesAsync(int mode, string? country = null);

    /// <summary>
    /// </summary>
    /// <param name="mode"></param>
    /// <returns>
    ///  A dictionary with the keys equal to the 'bucket' of rating displayed
    ///  in the histogram, and the values being how many players have ratings within the buckets.
    /// </returns>
    Task<IDictionary<int, int>> GetHistogramAsync(int mode);
}
