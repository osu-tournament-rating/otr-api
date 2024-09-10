using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Describes a single change to a PlayerRating
/// </summary>
public class RatingAdjustmentDTO
{
    /// <summary>
    /// The type of event that caused the adjustment
    /// </summary>
    public RatingAdjustmentType AdjustmentType { get; init; }

    /// <summary>
    /// Timestamp of when the adjustment was applied
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Rating before the adjustment
    /// </summary>
    public double RatingBefore { get; init; }

    /// <summary>
    /// Rating after the adjustment
    /// </summary>
    public double RatingAfter { get; init; }

    /// <summary>
    /// Total change in rating
    /// </summary>
    public double RatingDelta { get; init; }

    /// <summary>
    /// Rating volatility before the adjustment
    /// </summary>
    public double VolatilityBefore { get; init; }

    /// <summary>
    /// Rating volatility after the adjustment
    /// </summary>
    public double VolatilityAfter { get; init; }

    /// <summary>
    /// Total change in rating volatility
    /// </summary>
    public double VolatilityDelta { get; init; }

    /// <summary>
    /// Id of the match the adjustment was created for if available
    /// </summary>
    public int? MatchId { get; init; }
}
