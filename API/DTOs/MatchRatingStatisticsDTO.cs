namespace API.DTOs;

/// <summary>
/// Used by the API to POST match rating statistics.
/// Also used for GET match rating statistics.
/// </summary>
public class MatchRatingStatisticsDTO
{
	public int PlayerId { get; set; }
	public int MatchId { get; set; }
	public double MatchCost { get; set; }
	public double RatingBefore { get; set; }
	public double RatingAfter { get; set; }
	public double RatingChange { get; set; }
	public double VolatilityBefore { get; set; }
	public double VolatilityAfter { get; set; }
	public double VolatilityChange { get; set; }
	public int GlobalRankBefore { get; set; }
	public int GlobalRankAfter { get; set; }
	public int GlobalRankChange { get; set; }
	public int CountryRankBefore { get; set; }
	public int CountryRankAfter { get; set; }
	public int CountryRankChange { get; set; }
	public double PercentileBefore { get; set; }
	public double PercentileAfter { get; set; }
	public double PercentileChange { get; set; }
	public double? AverageTeammateRating { get; set; }
	public double? AverageOpponentRating { get; set; }
}