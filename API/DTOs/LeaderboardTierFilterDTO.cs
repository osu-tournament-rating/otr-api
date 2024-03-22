namespace API.DTOs;

/// <summary>
/// A collection of booleans representing which tiers to filter.
///
/// False = Default, no behavioral change
/// True = Explicitly included
///
/// If everything is false *and* if everything is true, the leaderboard will return
/// in its default state. If everything is false except for Bronze and Emerald,
/// then only Bronze and Emerald players will show up in the leaderboard.
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
