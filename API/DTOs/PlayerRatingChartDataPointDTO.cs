using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Represents a data point used to construct a rating chart for a player
/// </summary>
public class PlayerRatingChartDataPointDTO
{
    /// <summary>
    /// Match name, if applicable
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Match id, if applicable
    /// </summary>
    public int? MatchId { get; init; }

    /// <summary>
    /// osu! match id, if applicable
    /// </summary>
    public long? MatchOsuId { get; init; }

    /// <summary>
    /// Match cost of the player, if applicable
    /// </summary>
    public double? MatchCost { get; init; }

    /// <summary>
    /// Rating of the player before the adjustment
    /// </summary>
    public double RatingBefore { get; init; }

    /// <summary>
    /// Rating of the player after the adjustment
    /// </summary>
    public double RatingAfter { get; init; }

    /// <summary>
    /// Volatility of the player before this match occurred
    /// </summary>
    public double VolatilityBefore { get; init; }

    /// <summary>
    /// Volatility of the player after this adjustment
    /// </summary>
    public double VolatilityAfter { get; init; }

    /// <summary>
    /// Difference in rating between now and the previous adjustment
    /// </summary>
    public double RatingChange => RatingAfter - RatingBefore;

    /// <summary>
    /// Difference in volatility between now and the previous adjustment
    /// </summary>
    public double VolatilityChange => VolatilityAfter - VolatilityBefore;

    /// <summary>
    /// Ruleset of the adjustment
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Adjustment type
    /// </summary>
    public RatingAdjustmentType RatingAdjustmentType { get; init; }

    /// <summary>
    /// Match start time
    /// </summary>
    public DateTime? Timestamp { get; init; }
}
