using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// DTO for comparing two players' stats, assuming they have faced off against each other
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class PlayerOpponentComparisonDTO
{
    /// <summary>
    /// The id of the player who is being compared against the opponent
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// The id of the opponent
    /// </summary>
    public int OpponentId { get; set; }

    /// <summary>
    /// Across how many matches has this matchup occurred?
    /// </summary>
    public int MatchesPlayed { get; set; }

    /// <summary>
    /// How many matches has the player won against the opponent?
    /// </summary>
    public int MatchesWon { get; set; }

    /// <summary>
    /// The amount of matches the opponent has won against the player (player has lost N matches to opponent)
    /// </summary>
    public int MatchesLost { get; set; }

    /// <summary>
    /// How many games have these two faced off against each other?
    /// </summary>
    public int GamesPlayed { get; set; }

    /// <summary>
    /// How often does the player win against the opponent?
    /// </summary>
    public double WinRate { get; set; }

    /// <summary>
    /// How much rating has been gained or lost as a result of facing the opponent?
    /// </summary>
    public double RatingDelta { get; set; }
}
