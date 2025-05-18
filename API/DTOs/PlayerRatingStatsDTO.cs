using API.Utilities;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Describes tournament rating based information for a player in a ruleset with additional statistics
/// </summary>
/// <remarks>If filtered by time, all fields in this class will change.</remarks>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PlayerRatingStatsDTO : PlayerRatingDTO
{
    /// <summary>
    /// Total number of tournaments played
    /// </summary>
    public int TournamentsPlayed { get; set; }

    /// <summary>
    /// Total number of matches played
    /// </summary>
    public int MatchesPlayed { get; set; }

    /// <summary>
    /// Match win rate
    /// </summary>
    public double WinRate { get; set; }

    /// <summary>
    /// Rating tier progress information
    /// </summary>
    public required TierProgressDTO TierProgress { get; set; }

    /// <summary>
    /// A collection of adjustments that describe the changes resulting in the final rating
    /// </summary>
    public ICollection<RatingAdjustmentDTO> Adjustments { get; set; } = [];

    /// <summary>
    /// Denotes the current rating as being provisional
    /// </summary>
    public bool IsProvisional => RatingUtils.IsProvisional(Volatility, MatchesPlayed, TournamentsPlayed);
}
