namespace API.DTOs;

/// <summary>
/// An individual data point for the chart above the main leaderboard, only for logged in users.
/// Subset of MatchRatingStats.
///
/// Rank can be the global or country rank, depending on how this was requested
/// </summary>
public class RankChartDataPointDTO
{
	public string TournamentName { get; set; } = null!;
	public string MatchName { get; set; } = null!;
	public int Rank { get; set; }
	public int RankChange { get; set; }
}