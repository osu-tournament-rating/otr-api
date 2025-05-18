using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents an aggregate of match statistics for a player during a period of time
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class AggregatePlayerMatchStatsDTO
{
    /// <summary>
    /// The player's average match cost during the period
    /// </summary>
    public double AverageMatchCostAggregate { get; set; }

    /// <summary>
    /// The peak rating achieved by the player during the period
    /// </summary>
    public double HighestRating { get; set; }

    /// <summary>
    /// The amount of rating gained from the start of the period to the end of the period
    /// </summary>
    public double RatingGained { get; set; }

    /// <summary>
    /// The amount of games won during the period
    /// </summary>
    public int GamesWon { get; set; }

    /// <summary>
    /// The amount of games lost during the period
    /// </summary>
    public int GamesLost { get; set; }

    /// <summary>
    /// The amount of games played during the period
    /// </summary>
    public int GamesPlayed { get; set; }

    /// <summary>
    /// The amount of matches won during the period
    /// </summary>
    public int MatchesWon { get; set; }

    /// <summary>
    /// The amount of matches lost during the period
    /// </summary>
    public int MatchesLost { get; set; }

    /// <summary>
    /// The amount of matches played during the period
    /// </summary>
    public int MatchesPlayed => MatchesWon + MatchesLost;

    /// <summary>
    /// A value between 0 and 1 representing the player's game win rate during the period
    /// </summary>
    public double GameWinRate => GamesPlayed == 0 ? 0 : (double)GamesWon / GamesPlayed;

    /// <summary>
    /// A value between 0 and 1 representing the player's match win rate during the period
    /// </summary>
    public double MatchWinRate => MatchesPlayed == 0 ? 0 : (double)MatchesWon / MatchesPlayed;

    /// <summary>
    /// The most amount of matches won in a row during the period
    /// </summary>
    public int BestWinStreak { get; set; }

    /// <summary>
    /// Across all matches the player has played in, the average score across the entire lobby. This average includes
    /// scores for games the player may have not been in for
    /// </summary>
    public double MatchAverageScoreAggregate { get; set; }

    /// <summary>
    /// Across all matches the player has played in, the average miss count of the lobby, across all games in that match
    /// </summary>
    public double MatchAverageMissesAggregate { get; set; }

    /// <summary>
    /// Across all matches the player has played in, the average accuracy of the lobby, across all games in that match
    /// </summary>
    public double MatchAverageAccuracyAggregate { get; set; }

    /// <summary>
    /// The amount of maps the player participates in, on average.
    /// </summary>
    public double AverageGamesPlayedAggregate { get; set; }

    /// <summary>
    /// The average lobby ranking the player has on maps they participate in.
    /// A top-score is 1, bottom score would be team size * 2
    /// </summary>
    public double AveragePlacingAggregate { get; set; }

    /// <summary>
    /// The beginning of the period for which the statistics are calculated.
    /// </summary>
    public DateTime? PeriodStart { get; set; }

    /// <summary>
    /// The end of the period for which the statistics are calculated.
    /// </summary>
    public DateTime? PeriodEnd { get; set; }
}
