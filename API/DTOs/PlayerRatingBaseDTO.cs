using Database.Enums;

namespace API.DTOs;

/// <summary>
/// TBD
/// </summary>
public class PlayerRatingBaseDTO
{
    public Ruleset Ruleset { get; set; }

    public double Rating { get; set; }

    public double Volatility { get; set; }

    public double Percentile { get; set; }

    public int GlobalRank { get; set; }

    public int CountryRank { get; set; }

    public int PlayerId { get; set; }

    public ICollection<RatingAdjustmentDTO> Adjustments { get; set; } = new List<RatingAdjustmentDTO>();
}
