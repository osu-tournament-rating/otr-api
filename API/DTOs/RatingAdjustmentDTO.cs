using Database.Enums;

namespace API.DTOs;

/// <summary>
/// TBD
/// </summary>
public class RatingAdjustmentDTO
{
    public RatingAdjustmentType AdjustmentType { get; init; }

    public DateTime Timestamp { get; init; }

    public double RatingBefore { get; init; }

    public double RatingAfter { get; init; }

    public double VolatilityBefore { get; init; }

    public double VolatilityAfter { get; init; }

    public int? MatchId { get; init; }
}
