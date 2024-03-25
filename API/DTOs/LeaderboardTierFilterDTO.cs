namespace API.DTOs;

/// <summary>
/// A collection of booleans representing which tiers to filter.
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
    public bool FilterBronze { get; set; }
    public bool FilterSilver { get; set; }
    public bool FilterGold { get; set; }
    public bool FilterPlatinum { get; set; }
    public bool FilterEmerald { get; set; }
    public bool FilterDiamond { get; set; }
    public bool FilterMaster { get; set; }
    public bool FilterGrandmaster { get; set; }
    public bool FilterEliteGrandmaster { get; set; }
}
