namespace Database.Models;

/// <summary>
/// Represents a filter for querying leaderboard data.
/// </summary>
public class LeaderboardFilter
{
    /// <summary>
    /// The minimum rank to include in the results. If null, no minimum rank is applied.
    /// </summary>
    public int? MinRank { get; set; }

    /// <summary>
    /// The maximum rank to include in the results. If null, no maximum rank is applied.
    /// </summary>
    public int? MaxRank { get; set; }

    /// <summary>
    /// The minimum rating to include in the results. If null, no minimum rating is applied.
    /// </summary>
    public int? MinRating { get; set; }

    /// <summary>
    /// The maximum rating to include in the results. If null, no maximum rating is applied.
    /// </summary>
    public int? MaxRating { get; set; }

    /// <summary>
    /// The minimum number of matches to include in the results. If null, no minimum matches are applied.
    /// </summary>
    public int? MinMatches { get; set; }

    /// <summary>
    /// The maximum number of matches to include in the results. If null, no maximum matches are applied.
    /// </summary>
    public int? MaxMatches { get; set; }

    /// <summary>
    /// Filters based on leaderboard tiers. If null, no tier-based filtering is applied.
    /// </summary>
    public LeaderboardTierFilter? TierFilters { get; set; }
}
