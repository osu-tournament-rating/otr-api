namespace API.DTOs;

public class Unmapped_PlayerStatisticsDTO
{
	/// <summary>
	/// The current ranking of the user, e.g. Platinum, Diamond
	/// </summary>
	public string Ranking { get; set; } = null!;
	/// <summary>
	/// The next ranking the user can achieve, e.g. Diamond, Master
	/// </summary>
	public string? NextRanking { get; set; }
	/// <summary>
	/// The current rating of the user for the selected mode
	/// </summary>
	public int Rating { get; set; }
	/// <summary>
	/// Global o!TR rank of the user for the selected mode
	/// </summary>
	public int GlobalRank { get; set; }
	public int CountryRank { get; set; }
	public double Percentile { get; set; }
	public int RatingDelta { get; set; }
	public int RatingForNextRank { get; set; }
	public int HighestRating { get; set; }
	public int MatchesPlayed { get; set; }
	public int GamesPlayed { get; set; }
	public int MatchesWon { get; set; }
	public int MatchesLost { get; set; }
	public int GamesWon { get; set; }
	public int GamesLost { get; set; }
	public int AverageOpponentRating { get; set; }
	public int AverageTeammateRating { get; set; }
	public int BestWinStreak { get; set; }
	public int MatchAverageScore { get; set; }
	public int MatchAverageMisses { get; set; }
	public int MatchAverageAccuracy { get; set; }
	public int MatchAverageMapsPlayed { get; set; }
	public int MapAverageScore { get; set; }
	public int MapAverageMisses { get; set; }
	public int MapAverageAccuracy { get; set; }
	public int MapAveragePlacing { get; set; }
	public int PlayedHR { get; set; }
	public int PlayedHD { get; set; }
	public int PlayedDT { get; set; }
	public string? MostPlayedTeammateName { get; set; }
	public string? BestPerformingTeammate { get; set; }
	public string? WorstPerformingTeammate { get; set; }
	public string? MostPlayedOpponent { get; set; }
	public string? BestPerformingOpponent { get; set; }
	public string? WorstPerformingOpponent { get; set; }
	/// <summary>
	/// On the frontend, say the user selects to view a 30 day chart.
	/// We show the rating gained or lost since the start of that period (30
	/// days ago to now)
	/// </summary>
	public int RatingGainedSincePeriod { get; set; }
	public bool IsRatingPositiveTrend { get; set; }
}