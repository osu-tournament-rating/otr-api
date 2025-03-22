namespace Database.Entities;

/// <summary>
/// Describes the performance of a <see cref="Entities.Player"/> over all <see cref="Match"/>es
/// in a <see cref="Entities.Tournament"/>
/// </summary>
public class PlayerTournamentStats : EntityBase
{
    /// <summary>
    /// Average change in rating
    /// </summary>
    public double AverageRatingDelta { get; set; }

    /// <summary>
    /// Average match cost
    /// </summary>
    public double AverageMatchCost { get; set; }

    /// <summary>
    /// Average score
    /// </summary>
    public int AverageScore { get; set; }

    /// <summary>
    /// Average placement
    /// </summary>
    public double AveragePlacement { get; set; }

    /// <summary>
    /// Average accuracy
    /// </summary>
    public double AverageAccuracy { get; set; }

    /// <summary>
    /// Total number of <see cref="Match"/>es played
    /// </summary>
    public int MatchesPlayed { get; set; }

    /// <summary>
    /// Total number of <see cref="Match"/>es won
    /// </summary>
    public int MatchesWon { get; set; }

    /// <summary>
    /// Total number of <see cref="Match"/>es lost
    /// </summary>
    public int MatchesLost { get; set; }

    /// <summary>
    /// Total number of <see cref="Game"/>s played
    /// </summary>
    public int GamesPlayed { get; set; }

    /// <summary>
    /// Total number of <see cref="Game"/>s won
    /// </summary>
    public int GamesWon { get; set; }

    /// <summary>
    /// Total number of <see cref="Game"/>s lost
    /// </summary>
    public int GamesLost { get; set; }

    /// <summary>
    /// The win rate across all matches
    /// </summary>
    public double MatchWinRate => MatchesWon / (double)MatchesPlayed;

    /// <summary>
    /// Ids of all <see cref="Entities.Player"/>s that appeared on the <see cref="Player"/>'s team
    /// </summary>
    public int[] TeammateIds { get; set; } = [];

    /// <summary>
    /// Id of the <see cref="Player"/> the <see cref="PlayerTournamentStats"/> was generated for
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Player"/> the <see cref="PlayerTournamentStats"/> was generated for
    /// </summary>
    public Player Player { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="Tournament"/> the <see cref="PlayerTournamentStats"/> was generated for
    /// </summary>
    public int TournamentId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Tournament"/> the <see cref="PlayerTournamentStats"/> was generated for
    /// </summary>
    public Tournament Tournament { get; set; } = null!;
}
