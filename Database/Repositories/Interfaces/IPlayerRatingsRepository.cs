using Common.Enums;
using Common.Enums.Enums;
using Database.Entities.Processor;

namespace Database.Repositories.Interfaces;

public interface IPlayerRatingsRepository : IRepository<PlayerRating>
{
    /// <summary>
    /// Get a rating for a given player and <see cref="Ruleset" />.
    /// </summary>
    /// <param name="playerId">Player id</param>
    /// <param name="ruleset">Ruleset</param>
    /// <returns>
    /// A <see cref="PlayerRating" /> for the given playerId and <see cref="Ruleset" />,
    /// or null if not found
    /// </returns>
    Task<PlayerRating?> GetAsync(int playerId, Ruleset ruleset, bool includeAdjustments = false);

    /// <summary>
    /// Get a collection of PlayerRatings with extensive filtering support.
    /// </summary>
    /// <param name="page">The one-indexed page number to retrieve.</param>
    /// <param name="pageSize">The number of elements to include on each page.</param>
    /// <param name="ruleset">The ruleset to filter the leaderboard by.</param>
    /// <param name="country">An optional country code to filter the leaderboard by. If not provided, the global leaderboard is returned.</param>
    /// <param name="minRank">The minimum osu! rank (inclusive) to filter players by.</param>
    /// <param name="maxRank">The maximum osu! rank (inclusive) to filter players by.</param>
    /// <param name="minRating">The minimum rating (inclusive) to filter players by.</param>
    /// <param name="maxRating">The maximum rating (inclusive) to filter players by.</param>
    /// <param name="minMatches">The minimum number of matches (inclusive) to filter players by.</param>
    /// <param name="maxMatches">The maximum number of matches (inclusive) to filter players by.</param>
    /// <param name="minWinRate">The minimum win rate (inclusive) to filter players by.</param>
    /// <param name="maxWinRate">The maximum win rate (inclusive) to filter players by.</param>
    /// <param name="bronze">Include bronze-tier players in the results.</param>
    /// <param name="silver">Include silver-tier players in the results.</param>
    /// <param name="gold">Include gold-tier players in the results.</param>
    /// <param name="platinum">Include platinum-tier players in the results.</param>
    /// <param name="emerald">Include emerald-tier players in the results.</param>
    /// <param name="diamond">Include diamond-tier players in the results.</param>
    /// <param name="master">Include master-tier players in the results.</param>
    /// <param name="grandmaster">Include grandmaster-tier players in the results.</param>
    /// <param name="eliteGrandmaster">Include elite grandmaster-tier players in the results.</param>
    /// <remarks>Used to generate an optionally filtered leaderboard</remarks>
    /// <returns>A collection containing up to <see cref="pageSize"/> <see cref="PlayerRating"/> objects without any adjustments.</returns>
    Task<IList<PlayerRating>> GetAsync(int page = 1, int pageSize = 25, Ruleset ruleset = Ruleset.Osu,
        string? country = null, int? minRank = null, int? maxRank = null, int? minRating = null, int? maxRating = null,
        int? minMatches = null, int? maxMatches = null, double? minWinRate = null, double? maxWinRate = null,
        bool bronze = false, bool silver = false, bool gold = false, bool platinum = false,
        bool emerald = false, bool diamond = false, bool master = false, bool grandmaster = false, bool eliteGrandmaster = false);

    /// <summary>
    /// Counts the number of pages which would have leaderboard content if navigated to
    /// </summary>
    /// <param name="pageSize">The number of elements to include on each page.</param>
    /// <param name="ruleset">The ruleset to filter the leaderboard by.</param>
    /// <param name="country">An optional country code to filter the leaderboard by. If not provided, the global leaderboard is returned.</param>
    /// <param name="minRank">The minimum osu! rank (inclusive) to filter players by.</param>
    /// <param name="maxRank">The maximum osu! rank (inclusive) to filter players by.</param>
    /// <param name="minRating">The minimum rating (inclusive) to filter players by.</param>
    /// <param name="maxRating">The maximum rating (inclusive) to filter players by.</param>
    /// <param name="minMatches">The minimum number of matches (inclusive) to filter players by.</param>
    /// <param name="maxMatches">The maximum number of matches (inclusive) to filter players by.</param>
    /// <param name="minWinRate">The minimum win rate (inclusive) to filter players by.</param>
    /// <param name="maxWinRate">The maximum win rate (inclusive) to filter players by.</param>
    /// <param name="bronze">Include bronze-tier players in the results.</param>
    /// <param name="silver">Include silver-tier players in the results.</param>
    /// <param name="gold">Include gold-tier players in the results.</param>
    /// <param name="platinum">Include platinum-tier players in the results.</param>
    /// <param name="emerald">Include emerald-tier players in the results.</param>
    /// <param name="diamond">Include diamond-tier players in the results.</param>
    /// <param name="master">Include master-tier players in the results.</param>
    /// <param name="grandmaster">Include grandmaster-tier players in the results.</param>
    /// <param name="eliteGrandmaster">Include elite grandmaster-tier players in the results.</param>
    /// <remarks>Used to generate an optionally filtered leaderboard</remarks>
    /// <returns>A collection containing up to <see cref="pageSize"/> <see cref="PlayerRating"/> objects without any adjustments.</returns>
    Task<int> PageCountAsync(int pageSize = 25, Ruleset ruleset = Ruleset.Osu,
        string? country = null, int? minRank = null, int? maxRank = null, int? minRating = null, int? maxRating = null,
        int? minMatches = null, int? maxMatches = null, double? minWinRate = null, double? maxWinRate = null,
        bool bronze = false, bool silver = false, bool gold = false, bool platinum = false,
        bool emerald = false, bool diamond = false, bool master = false, bool grandmaster = false, bool eliteGrandmaster = false);

    /// <summary>
    /// Get a list of rulesets for which the player has a rating
    /// </summary>
    /// <param name="playerId">Player id</param>
    /// <returns>All rulesets which the player has a rating for</returns>
    Task<IList<Ruleset>> GetActiveRulesetsAsync(int playerId);

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
