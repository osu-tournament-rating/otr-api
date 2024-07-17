using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

/// <summary>
/// Describes the performance of a <see cref="Entities.Player"/> over all <see cref="Match"/>es
/// in a <see cref="Entities.Tournament"/>
/// </summary>
[Table("player_tournament_stats")]
public class PlayerTournamentStats : EntityBase
{
    /// <summary>
    /// Average change in rating
    /// </summary>
    [Column("average_rating_delta")]
    public double AverageRatingDelta { get; set; }

    /// <summary>
    /// Average match cost
    /// </summary>
    [Column("average_match_cost")]
    public double AverageMatchCost { get; set; }

    /// <summary>
    /// Average score
    /// </summary>
    [Column("average_score")]
    public int AverageScore { get; set; }

    /// <summary>
    /// Average placement
    /// </summary>
    [Column("average_placement")]
    public double AveragePlacement { get; set; }

    /// <summary>
    /// Average accuracy
    /// </summary>
    [Column("average_accuracy")]
    public double AverageAccuracy { get; set; }

    /// <summary>
    /// Total number of <see cref="Match"/>es played
    /// </summary>
    [Column("matches_played")]
    public int MatchesPlayed { get; set; }

    /// <summary>
    /// Total number of <see cref="Match"/>es won
    /// </summary>
    [Column("matches_won")]
    public int MatchesWon { get; set; }

    /// <summary>
    /// Total number of <see cref="Match"/>es lost
    /// </summary>
    [Column("matches_lost")]
    public int MatchesLost { get; set; }

    /// <summary>
    /// Total number of <see cref="Game"/>s played
    /// </summary>
    [Column("games_played")]
    public int GamesPlayed { get; set; }

    /// <summary>
    /// Total number of <see cref="Game"/>s won
    /// </summary>
    [Column("games_won")]
    public int GamesWon { get; set; }

    /// <summary>
    /// Total number of <see cref="Game"/>s lost
    /// </summary>
    [Column("games_lost")]
    public int GamesLost { get; set; }

    /// <summary>
    /// Ids of all <see cref="Entities.Player"/>s that appeared on the <see cref="Player"/>'s team
    /// </summary>
    [Column("teammate_ids")]
    public int[] TeammateIds { get; set; } = [];

    /// <summary>
    /// Id of the <see cref="Player"/> the <see cref="PlayerTournamentStats"/> was generated for
    /// </summary>
    [Column("player_id")]
    public int PlayerId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Player"/> the <see cref="PlayerTournamentStats"/> was generated for
    /// </summary>
    public Player Player { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="Tournament"/> the <see cref="PlayerTournamentStats"/> was generated for
    /// </summary>
    [Column("tournament_id")]
    public int TournamentId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Tournament"/> the <see cref="PlayerTournamentStats"/> was generated for
    /// </summary>
    public Tournament Tournament { get; set; } = null!;
}
