using System.ComponentModel.DataAnnotations.Schema;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Represents a record of who played in a match and who won or lost.
/// </summary>
[Table("match_win_records")]
public class MatchWinRecord
{
    /// <summary>
    /// The id of this record
    /// </summary>
    [Column("id")]
    public int Id { get; set; }
    /// <summary>
    /// The id of the match this record is for
    /// </summary>
    [Column("match_id")]
    public int MatchId { get; set; }
    /// <summary>
    /// The ids of the players on the losing team
    /// </summary>
    [Column("loser_roster")]
    public int[] LoserRoster { get; set; } = [];
    /// <summary>
    /// The ids of the players on the winning team
    /// </summary>
    [Column("winner_roster")]
    public int[] WinnerRoster { get; set; } = [];
    /// <summary>
    /// The points the winning team scored
    /// </summary>
    [Column("winner_points")]
    public int WinnerPoints { get; set; }
    /// <summary>
    /// The points the losing team scored
    /// </summary>
    [Column("loser_points")]
    public int LoserPoints { get; set; }
    /// <summary>
    /// The team that won the match
    /// </summary>
    [Column("winner_team")]
    public Team? WinnerTeam { get; set; }
    /// <summary>
    /// The team that lost the match
    /// </summary>
    [Column("loser_team")]
    public Team? LoserTeam { get; set; }
    /// <summary>
    /// The type of match this record is for (team or head-to-head)
    /// </summary>
    [Column("match_type")]
    public OsuMatchType? MatchType { get; set; }
    /// <summary>
    /// The <see cref="Match"/> this record belongs to
    /// </summary>
    [InverseProperty("WinRecord")]
    public Match Match { get; set; } = null!;
}
