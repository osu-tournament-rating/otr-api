using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents comprehensive statistics for a match including rating adjustments and player statistics
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MatchStatisticsDTO
{
    /// <summary>
    /// The match ID these statistics belong to
    /// </summary>
    public int MatchId { get; init; }

    /// <summary>
    /// Collection of all rating adjustments for all players in the match
    /// </summary>
    public ICollection<RatingAdjustmentDTO> RatingAdjustments { get; init; } = [];

    /// <summary>
    /// Collection of all player match statistics for all players in the match
    /// </summary>
    public ICollection<PlayerMatchStatsDTO> PlayerMatchStats { get; init; } = [];
}

