namespace API.DTOs;

/// <summary>
/// Filters for the leaderboard
/// </summary>
public class LeaderboardFilterDTO
{
    /// <summary>
    /// The "better" inclusive bound (ranges from 1+)
    /// </summary>
    public int? MinRank { get; set; }

    /// <summary>
    /// The "worse" inclusive bound (ranges from 1+)
    /// </summary>
    public int? MaxRank { get; set; }

    /// <summary>
    /// The lower-performing rating bound (ranges from 100+)
    /// </summary>
    public int? MinRating { get; set; }

    /// <summary>
    /// The higher-performing rating bound (ranges from 100+)
    /// </summary>
    public int? MaxRating { get; set; }

    /// <summary>
    /// The minimum number of matches played (ranges from 1-10000)
    /// </summary>
    public int? MinMatches { get; set; }

    /// <summary>
    /// The maximum number of matches played (ranges from 1-10000)
    /// </summary>
    public int? MaxMatches { get; set; }

    /// <summary>
    /// Ranges from 0.00-1.00
    /// </summary>
    public double? MinWinrate { get; set; }

    /// <summary>
    /// Ranges from 0.00-1.00
    /// </summary>
    public double? MaxWinrate { get; set; }

    /// <summary>
    /// A collection of optional filters for tiers
    /// </summary>
    public LeaderboardTierFilterDTO? TierFilters { get; set; }
}
