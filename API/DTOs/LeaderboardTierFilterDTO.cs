using Microsoft.AspNetCore.Mvc;

namespace API.DTOs;

/// <summary>
/// A collection of booleans representing which tiers to filter on the leaderboard.
///
/// False = Default, no behavioral change
/// True = Explicitly included in leaderboard results
///
/// If *all* tiers are set to false, or all tiers are set to true, the leaderboard will return
/// as if no tier filters were applied.
///
/// For example, if Bronze and Emerald are true and everything else is false,
/// then only Bronze and Emerald players will show up in the leaderboard
/// (specifically, Bronze III-I and Emerald III-I)
/// </summary>
public class LeaderboardTierFilterDTO
{
    /// <summary>
    /// Explicitly include bronze players
    /// </summary>
    [FromQuery(Name = "bronze")]
    public bool FilterBronze { get; init; }

    /// <summary>
    /// Explicitly include silver players
    /// </summary>
    [FromQuery(Name = "silver")]
    public bool FilterSilver { get; init; }

    /// <summary>
    /// Explicitly include gold players
    /// </summary>
    [FromQuery(Name = "gold")]
    public bool FilterGold { get; init; }

    /// <summary>
    /// Explicitly include platinum players
    /// </summary>
    [FromQuery(Name = "platinum")]
    public bool FilterPlatinum { get; init; }

    /// <summary>
    /// Explicitly include emerald players
    /// </summary>
    [FromQuery(Name = "emerald")]
    public bool FilterEmerald { get; init; }

    /// <summary>
    /// Explicitly include emerald players
    /// </summary>
    [FromQuery(Name = "diamond")]
    public bool FilterDiamond { get; init; }

    /// <summary>
    /// Explicitly include master players
    /// </summary>
    [FromQuery(Name = "master")]
    public bool FilterMaster { get; init; }

    /// <summary>
    /// Explicitly include grandmaster players
    /// </summary>
    [FromQuery(Name = "grandmaster")]
    public bool FilterGrandmaster { get; init; }

    /// <summary>
    /// Explicitly include elite grandmaster players
    /// </summary>
    [FromQuery(Name = "eliteGrandmaster")]
    public bool FilterEliteGrandmaster { get; init; }
}
