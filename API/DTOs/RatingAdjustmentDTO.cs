namespace API.DTOs;

/// <summary>
/// Used for fetching and posting rating adjustments
/// </summary>
public class RatingAdjustmentDTO
{
	public int PlayerId { get; set; }
	public int Mode { get; set; }
	public double RatingAdjustmentAmount { get; set; }
	public double VolatilityAdjustmentAmount { get; set; }
	public double RatingBefore { get; set; } 
	public double RatingAfter { get; set; }
	public double VolatilityBefore { get; set; }
	public double VolatilityAfter { get; set; }
	public int RatingAdjustmentType { get; set; }
	public DateTime Timestamp { get; set; }
}