using Common.Enums;

namespace API.DTOs;

/// <summary>
/// Represents counts of participation in games of differing mod combinations
/// </summary>
public class PlayerModStatsDTO
{
    /// <summary>
    /// The combination of mods used
    /// </summary>
    public Mods Mods { get; set; }

    /// <summary>
    /// The number of times the player participated with this mod combination
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// The average score achieved by the player with this mod combination.
    /// </summary>
    public int AverageScore { get; set; }
}
