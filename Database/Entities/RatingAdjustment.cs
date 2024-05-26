using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("rating_adjustments")]
public class RatingAdjustment
{
    [Column("id")]
    public int Id { get; set; }

    [Column("player_id")]
    public int PlayerId { get; set; }

    /// <summary>
    /// The mode of the rating / volatility we are modifying
    /// </summary>
    [Column("mode")]
    public int Mode { get; set; }

    [Column("rating_adjustment_amount")]
    public double RatingAdjustmentAmount { get; set; }

    [Column("volatility_adjustment_amount")]
    public double VolatilityAdjustmentAmount { get; set; }

    [Column("rating_before")]
    public double RatingBefore { get; set; }

    [Column("rating_after")]
    public double RatingAfter { get; set; }

    [Column("volatility_before")]
    public double VolatilityBefore { get; set; }

    [Column("volatility_after")]
    public double VolatilityAfter { get; set; }

    [Column("rating_adjustment_type")]
    public int RatingAdjustmentType { get; set; } // Maps to RatingAdjustmentType enum

    [Column("timestamp")]
    public DateTime Timestamp { get; set; }

    [InverseProperty("RatingAdjustments")]
    public virtual Player Player { get; set; } = null!;
}
