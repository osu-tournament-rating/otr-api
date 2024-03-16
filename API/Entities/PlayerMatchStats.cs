using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

/// <summary>
/// A set of statistics updated by the rating processor for each player.
/// This set of stats is for a single match. This is here to avoid duplicate data
/// in <see cref="PlayerGameStatistics"/>, as that data gets set on a per-game basis.
/// </summary>
[Table("player_match_stats")]
public class PlayerMatchStats
{
    [Column("id")]
    public int Id { get; set; }

    [Column("player_id")]
    public int PlayerId { get; set; }

    [Column("match_id")]
    public int MatchId { get; set; }

    [Column("won")]
    public bool Won { get; set; }

    [Column("average_score")]
    public int AverageScore { get; set; }

    [Column("average_misses")]
    public double AverageMisses { get; set; }

    [Column("average_accuracy")]
    public double AverageAccuracy { get; set; }

    [Column("games_played")]
    public int GamesPlayed { get; set; }

    [Column("average_placement")]
    public double AveragePlacement { get; set; }

    [Column("games_won")]
    public int GamesWon { get; set; }

    [Column("games_lost")]
    public int GamesLost { get; set; }

    [Column("teammate_ids")]
    public int[] TeammateIds { get; set; } = [];

    [Column("opponent_ids")]
    public int[] OpponentIds { get; set; } = [];

    [InverseProperty("MatchStats")]
    public virtual Player Player { get; set; } = null!;

    [InverseProperty("Stats")]
    public virtual Match Match { get; set; } = null!;
}
