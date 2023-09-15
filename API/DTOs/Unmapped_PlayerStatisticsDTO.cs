namespace API.DTOs;

public class Unmapped_PlayerStatisticsDTO
{
	/// <summary>
	/// e.g. Platinum, Diamond
	/// </summary>
	public string RankingClassName { get; set; } = null!;
	public string? NextRankingClassName { get; set; }
	public int Rating { get; set; }
	public int GlobalRank { get; set; }
	public int CountryRank { get; set; }
	public double Percentile { get; set; }
	public int RatingForNextRank { get; set; }
}