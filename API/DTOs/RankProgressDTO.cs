namespace API.DTOs;

public class RankProgressDTO
{
	public string CurrentTier { get; set; } = null!;
	public int? CurrentSubTier { get; set; }
	public double? RatingForNextTier { get; set; }
	public double? RatingForNextMajorTier { get; set; }
	public string? NextMajorTier { get; set; }
	public double? SubTierFillPercentage { get; set; }
	/// <summary>
	///  How far the frontend dashboard bar, covering all 3 subtiers, should be filled.
	/// </summary>
	public double? MajorTierFillPercentage { get; set; }
}