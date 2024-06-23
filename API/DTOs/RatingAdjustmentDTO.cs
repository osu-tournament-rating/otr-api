using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Used for fetching and posting rating adjustments
/// </summary>
public class RatingAdjustmentDTO
{
    public RatingAdjustmentType AdjustmentType { get; init; }

    public DateTime Timestamp { get; init; }

    public double RatingBefore { get; init; }

    public double RatingAfter { get; init; }

    public double RatingDelta { get; init; }

    public double VolatilityBefore { get; init; }

    public double VolatilityAfter { get; init; }

    public double VolatilityDelta { get; init; }

    public double PercentileBefore { get; init; }

    public double PercentileAfter { get; init; }

    public double PercentileDelta { get; init; }

    public int GlobalRankBefore { get; init; }

    public int GlobalRankAfter { get; init; }

    public int GlobalRankDelta { get; init; }

    public int CountryRankBefore { get; init; }

    public int CountryRankAfter { get; init; }

    public int CountryRankDelta { get; init; }

    public int? MatchId { get; init; }
}
