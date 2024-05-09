namespace API.DTOs;

/// <summary>
/// Represents a player's match stats
/// </summary>
public class PlayerMatchStatsDTO
{
    /// <summary>
    /// The id of the player
    /// </summary>
    public int PlayerId { get; set; }
    /// <summary>
    /// The id of the match
    /// </summary>
    public int MatchId { get; set; }
    /// <summary>
    /// Whether the player (or their team) won this match
    /// </summary>
    public bool Won { get; set; }
    /// <summary>
    /// The player's average score in this match
    /// </summary>
    public double AverageScore { get; set; }
    /// <summary>
    /// The player's average misses in this match
    /// </summary>
    public double AverageMisses { get; set; }
    /// <summary>
    /// The player's average accuracy in this match
    /// </summary>
    public double AverageAccuracy { get; set; }
    /// <summary>
    /// The player's average placement in this match
    /// </summary>
    public double AveragePlacement { get; set; }
    /// <summary>
    /// The number of games the player (or their team) won in the match.
    /// </summary>
    /// <remarks>
    /// The player must have participated in the game for this to count.
    /// If they were on the same team as the winner, but not in the lobby,
    /// this will not count towards the total.
    /// </remarks>
    public int GamesWon { get; set; }
    /// <summary>
    /// The number of games the player (or their team) lost in the match.
    /// </summary>
    /// <remarks>
    ///  The player must have participated in the game for this to count.
    ///  If they were on the same team as the loser, but not in the lobby,
    ///  this will not count towards the total.
    /// </remarks>
    public int GamesLost { get; set; }
    /// <summary>
    /// The total number of games the player participated in during this match
    /// </summary>
    public int GamesPlayed { get; set; }
    /// <summary>
    /// A unique list of player ids that were on the same team as the player in this match.
    /// </summary>
    /// <remarks>
    /// Does not include the player's id. Should be empty for a 1v1.
    public int[] TeammateIds { get; set; } = [];
    /// <summary>
    /// A unique list of player ids that were on the opposing team as the player in this match.
    /// </summary>
    /// <remarks>
    /// In a 1v1, this would only contain the opponent's id.
    /// </remarks>
    public int[] OpponentIds { get; set; } = [];
}
