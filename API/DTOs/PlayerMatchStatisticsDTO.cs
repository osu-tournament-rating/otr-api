using API.Utilities;

namespace API.DTOs;

/// <summary>
/// An aggregate of match statistics for a player during a period of time.
/// </summary>
public class PlayerMatchStatisticsDTO
{
	/// <summary>
	/// The peak rating achieved by the player during the period.
	/// </summary>
	public int HighestRating { get; set; }
	/// <summary>
	/// The peak global rank achieved by the player during the period.
	/// </summary>
	public int HighestGlobalRank { get; set; }
	/// <summary>
	/// The peak country rank achieved by the player during the period.
	/// </summary>
	public int HighestCountryRank { get; set; }
	/// <summary>
	/// The peak rating percentile achieved by the player during the period.
	/// </summary>
	public double HighestPercentile { get; set; }
	/// <summary>
	/// The amount of rating gained from the start of the period to the end of the period.
	/// </summary>
	public double RatingGained { get; set; }
	/// <summary>
	/// The amount of games won during the period.
	/// </summary>
	public int GamesWon { get; set; }
	/// <summary>
	/// The amount of games lost during the period.
	/// </summary>
	public int GamesLost { get; set; }
	/// <summary>
	/// The amount of matches won during the period.
	/// </summary>
	public int MatchesWon { get; set; }
	/// <summary>
	/// The amount of matches lost during the period.
	/// </summary>
	public int MatchesLost { get; set; }
	/// <summary>
	/// The amount of games played during the period.
	/// </summary>
	public int MatchesPlayed => MatchesWon + MatchesLost;
	/// <summary>
	/// A value between 0 and 1 representing the player's game win rate during the period.
	/// </summary>
	public double GameWinRate => GamesPlayed == 0 ? 0 : (double)GamesWon / GamesPlayed;
	/// <summary>
	/// A value between 0 and 1 representing the player's match win rate during the period.
	/// </summary>
	public double MatchWinRate => MatchesPlayed == 0 ? 0 : (double)MatchesWon / MatchesPlayed;
	/// <summary>
	/// The average rating of the player's teammates during the period. This average does not include the player's own rating.
	/// </summary>
	public double? AverageTeammateRating { get; set; }
	/// <summary>
	/// The average rating of the player's opponents during the period.
	/// </summary>
	public double? AverageOpponentRating { get; set; }
	/// <summary>
	/// The most amount of matches won in a row during the period.
	/// </summary>
	public int BestWinStreak { get; set; }
	/// <summary>
	/// Across all matches the player has played in, the average score across the entire lobby. This average includes
	/// scores for games the player may have not been in for.
	/// </summary>
	public double MatchAverageScoreAggregate { get; set; }
	/// <summary>
	/// Across all matches the player has played in, the average miss count of the lobby, across all games in that match.
	/// </summary>
	public double MatchAverageMissesAggregate { get; set; }
	/// <summary>
	/// Across all matches the player has played in, the average accuracy of the lobby, across all games in that match.
	/// </summary>
	public double MatchAverageAccuracyAggregate { get; set; }
	/// <summary>
	/// The amount of maps the player participates in, on average.
	/// </summary>
	public double AverageGamesPlayedAggregate { get; set; }
	/// <summary>
	/// The average lobby ranking the player has on maps they participate in.
	/// A top-score would be 1, bottom score would be team size * 2.
	/// </summary>
	public double AveragePlacingAggregate { get; set; }
	/// <summary>
	/// The name of the teammate the player has played with the most during the period.
	/// </summary>
	public string? MostPlayedTeammateName { get; set; }
	/// <summary>
	/// The name of the opponent the player has played against the most during the period.
	/// </summary>
	public string? MostPlayedOpponentName { get; set; }
	/// <summary>
	/// The name of the teammate who has the highest rating during the period.
	/// If the period is looking to the present, this will be the teammate with the highest current rating.
	/// </summary>
	// public string? BestTeammateName { get; set; }
	
	/// <summary>
	/// The beginning of the period for which the statistics are calculated.
	/// </summary>
	public DateTime PeriodStart { get; set; }
	/// <summary>
	/// The end of the period for which the statistics are calculated.
	/// </summary>
	public DateTime PeriodEnd { get; set; }
	/// <summary>
	/// The total number of games played in the period
	/// </summary>
	public int GamesPlayed => GamesWon + GamesLost;
}