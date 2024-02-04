namespace API.DTOs;

public class PlayerRatingChartDataPointDTO
{
	public string Name { get; set; } = string.Empty;
	public double RatingBefore { get; set; }
	public double RatingAfter { get; set; }
	public double VolatilityBefore { get; set; }
	public double VolatilityAfter { get; set; }
	public double RatingChange => RatingAfter - RatingBefore;
	public double VolatilityChange => VolatilityAfter - VolatilityBefore;
	public bool IsAdjustment { get; set; }
}