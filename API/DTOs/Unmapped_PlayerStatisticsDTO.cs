namespace API.DTOs;

public class Unmapped_PlayerStatisticsDTO
{
	/// <summary>
	/// e.g. Platinum, Diamond
	/// </summary>
	public string Ranking { get; set; } = null!;
	public string? NextRanking { get; set; }
	public int Rating { get; set; }
	public int GlobalRank { get; set; }
	public int CountryRank { get; set; }
	public double Percentile { get; set; }
	public int RatingDelta { get; set; }
	public int RatingForNextRank { get; set; }
	public int HighestRating { get; set; }
	public int HighestGlobalRank { get; set; }
	public double HighestPercentile { get; set; }
	public int MatchesPlayed { get; set; }
	public int GamesPlayed { get; set; }
	public int MatchesWon { get; set; }
	public int GamesWon { get; set; }
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
	public int PlayedNM { get; set; }
	public string? MostPlayedTeammateName { get; set; }
	public string? BestPerformingTeammate { get; set; }
	public string? WorstPerformingTeammate { get; set; }
	public string? MostPlayedOpponent { get; set; }
	public string? BestPerformingOpponent { get; set; }
	public string? WorstPerformingOpponent { get; set; }
	public int RatingGainedSincePeriod { get; set; }
	
	// Trend bools
	public bool IsRatingPositiveTrend { get; set; }
	public bool IsGlobalRankPositiveTrend { get; set; }
	public bool IsCountryRankPositiveTrend { get; set; }
	public bool IsPercentilePositiveTrend { get; set; }
}