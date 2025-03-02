using Common.Enums.Enums;

namespace API.DTOs;

/// <summary>
/// Describes tournament rating based information for a player in a ruleset that are current and not time specific
/// </summary>
public class PlayerRatingDTO
{
    /// <summary>
    /// Ruleset
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Rating
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// Rating volatility
    /// </summary>
    public double Volatility { get; set; }

    /// <summary>
    /// Global rating percentile
    /// </summary>
    public double Percentile { get; set; }

    /// <summary>
    /// Global rank
    /// </summary>
    public int GlobalRank { get; set; }

    /// <summary>
    /// Country rank
    /// </summary>
    public int CountryRank { get; set; }

    /// <summary>
    /// The player
    /// </summary>
    public PlayerCompactDTO Player { get; set; } = null!;

    /// <summary>
    /// A collection of adjustments that describe the changes resulting in the final rating
    /// </summary>
    public ICollection<RatingAdjustmentDTO> Adjustments { get; set; } = [];
}
