using API.Utilities;

namespace API.DTOs;

/// <summary>
/// DTO for the chart above the main leaderboard, only for logged in users
/// </summary>
public class LeaderboardPlayerChartDTO
{
	public int Rank { get; set; }
	public double Percentile { get; set; }
	public double Rating { get; set; }
	public int Matches { get; set; }
	public double Winrate { get; set; }
	public int HighestRank { get; set; }
	public string Tier => RatingUtils.GetTier(Rating);
	public PlayerRankChartDTO RankChart { get; set; } = new();
}