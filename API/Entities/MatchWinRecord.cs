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
	[Column("team1")]
	public int[] Team1 { get; set; } = Array.Empty<int>();
	[Column("team2")]
	public int[] Team2 { get; set; } = Array.Empty<int>();
	[Column("team1_points")]
	public int Team1Points { get; set; }
	[Column("team2_points")]
	public int Team2Points { get; set; }
	[Column("winner_team")]
	public int? WinnerTeam { get; set; }
	[Column("loser_team")]
	public int? LoserTeam { get; set; }
	[InverseProperty("WinRecord")]
	public virtual Match Match { get; set; }
}