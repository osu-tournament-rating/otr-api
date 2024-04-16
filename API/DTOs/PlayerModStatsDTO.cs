namespace API.DTOs;

/// <summary>
/// Represents counts of participation in games of differing mod combinations
/// </summary>
public class PlayerModStatsDTO
{
    /// <summary>
    /// Number of games played with no mods
    /// </summary>
    public ModStatsDTO? PlayedNM { get; set; }

    /// <summary>
    /// Number of games played with easy
    /// </summary>
    public ModStatsDTO? PlayedEZ { get; set; }

    /// <summary>
    /// Number of games played with half time
    /// </summary>
    public ModStatsDTO? PlayedHT { get; set; }

    /// <summary>
    /// Number of games played with hidden
    /// </summary>
    public ModStatsDTO? PlayedHD { get; set; }

    /// <summary>
    /// Number of games played with hard rock
    /// </summary>
    public ModStatsDTO? PlayedHR { get; set; }

    /// <summary>
    /// Number of games played with double time
    /// </summary>
    public ModStatsDTO? PlayedDT { get; set; }

    /// <summary>
    /// Number of games played with flashlight
    /// </summary>
    public ModStatsDTO? PlayedFL { get; set; }

    /// <summary>
    /// Number of games played with both hidden and hard rock
    /// </summary>
    public ModStatsDTO? PlayedHDHR { get; set; }

    /// <summary>
    /// Number of games played with both hidden and double time
    /// </summary>
    public ModStatsDTO? PlayedHDDT { get; set; }

    /// <summary>
    /// Number of games played with both hidden and easy
    /// </summary>
    public ModStatsDTO? PlayedHDEZ { get; set; }
}
