using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

/// <summary>
/// Represents a record of who played in a match and who won/lost.
/// </summary>
[Table("match_win_records")]
public class MatchWinRecord
{
    [Column("id")]
    public int Id { get; set; }

    [Column("match_id")]
    public int MatchId { get; set; }

    [Column("loser_roster")]
    public int[] LoserRoster { get; set; } = [];

    [Column("winner_roster")]
    public int[] WinnerRoster { get; set; } = [];

    [Column("winner_points")]
    public int WinnerPoints { get; set; }

    [Column("loser_points")]
    public int LoserPoints { get; set; }

    [Column("winner_team")]
    public int? WinnerTeam { get; set; }

    [Column("loser_team")]
    public int? LoserTeam { get; set; }

    [Column("match_type")]
    public Enums.MatchType? MatchType { get; set; }

    [InverseProperty("WinRecord")]
    public virtual Match Match { get; set; } = null!;
}
