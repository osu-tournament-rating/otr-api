namespace API.DTOs;

/// <summary>
/// Represents a data point used to construct a rating chart for a player
/// </summary>
public class PlayerRatingChartDataPointDTO
{
    /// <summary>
    /// Match name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Match id
    /// </summary>
    public int? MatchId { get; set; }

    /// <summary>
    /// osu! match id
    /// </summary>
    public long? MatchOsuId { get; set; }

    /// <summary>
    /// Match cost of the player
    /// </summary>
    public double? MatchCost { get; set; }

    /// <summary>
    /// Rating of the player before this match occurred
    /// </summary>
    public double RatingBefore { get; set; }

    /// <summary>
    /// Rating of the player after this match occurred
    /// </summary>
    public double RatingAfter { get; set; }

    /// <summary>
    /// Volatility of the player before this match occurred
    /// </summary>
    public double VolatilityBefore { get; set; }

    /// <summary>
    /// Volatility of the player after this match occurred
    /// </summary>
    public double VolatilityAfter { get; set; }

    /// <summary>
    /// Difference in rating for the player after this match occurred
    /// </summary>
    public double RatingChange => RatingAfter - RatingBefore;

    /// <summary>
    /// Difference in volatility for the player after this match occurred
    /// </summary>
    public double VolatilityChange => VolatilityAfter - VolatilityBefore;

    /// <summary>
    /// Indicates whether this data point is from a rating change that occurred outside of a match (i.e. decay)
    /// </summary>
    public bool IsAdjustment { get; set; }

    /// <summary>
    /// Match start time
    /// </summary>
    public DateTime? Timestamp { get; set; }
}
