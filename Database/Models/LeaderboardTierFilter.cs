namespace Database.Models;

/// <summary>
/// Represents a filter for leaderboard tiers, allowing selective inclusion or exclusion of specific tiers.
/// Each property corresponds to a tier and determines whether it should be included in the filtered results.
/// </summary>
public class LeaderboardTierFilter
{
    /// <summary>Indicates whether the Bronze tier should be included in the filter.</summary>
    public bool FilterBronze { get; set; }

    /// <summary>Indicates whether the Silver tier should be included in the filter.</summary>
    public bool FilterSilver { get; set; }

    /// <summary>Indicates whether the Gold tier should be included in the filter.</summary>
    public bool FilterGold { get; set; }

    /// <summary>Indicates whether the Platinum tier should be included in the filter.</summary>
    public bool FilterPlatinum { get; set; }

    /// <summary>Indicates whether the Emerald tier should be included in the filter.</summary>
    public bool FilterEmerald { get; set; }

    /// <summary>Indicates whether the Diamond tier should be included in the filter.</summary>
    public bool FilterDiamond { get; set; }

    /// <summary>Indicates whether the Master tier should be included in the filter.</summary>
    public bool FilterMaster { get; set; }

    /// <summary>Indicates whether the Grandmaster tier should be included in the filter.</summary>
    public bool FilterGrandmaster { get; set; }

    /// <summary>Indicates whether the Elite Grandmaster tier should be included in the filter.</summary>
    public bool FilterEliteGrandmaster { get; set; }
}
