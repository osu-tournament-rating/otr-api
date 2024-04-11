using API.Utilities;

namespace API.DTOs;

/// <summary>
/// Represents general statistics for a player that are current and not time specific
/// </summary>
public class BaseStatsDTO
{
    /// <summary>
    /// Id of the player
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// Current rating
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// Current rating volatility measure
    /// </summary>
    public double Volatility { get; set; }

    /// <summary>
    /// osu! ruleset the statistics are derived from
    /// </summary>
    public int Mode { get; set; }

    /// <summary>
    /// Current rating percentile
    /// </summary>
    public double Percentile { get; set; }

    /// <summary>
    /// Total number of matches played
    /// </summary>
    public int MatchesPlayed { get; set; }

    /// <summary>
    /// Current match win rate
    /// </summary>
    public double Winrate { get; set; }

    /// <summary>
    /// Highest o!tr global ranking
    /// </summary>
    public int HighestGlobalRank { get; set; }

    /// <summary>
    /// Current o!tr global rank
    /// </summary>
    public int GlobalRank { get; set; }

    /// <summary>
    /// Current o!tr country rank
    /// </summary>
    public int CountryRank { get; set; }

    /// <summary>
    /// Current average match cost
    /// </summary>
    public double AverageMatchCost { get; set; }

    /// <summary>
    /// Total number of tournaments played
    /// </summary>
    public int TournamentsPlayed { get; set; }

    /// <summary>
    /// Rating tier progress data
    /// </summary>
    public RankProgressDTO RankProgress { get; set; } = new();

    /// <summary>
    /// Denotes the current rating as being provisional
    /// </summary>
    public bool IsProvisional => RatingUtils.IsProvisional(Volatility, MatchesPlayed, TournamentsPlayed);
}
