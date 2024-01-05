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

	// Team arrays can represent individual players in individual matches (head to head)
	[Column("team_blue")]
	public int[] TeamBlue { get; set; } = Array.Empty<int>();

	[Column("team_red")]
	public int[] TeamRed { get; set; } = Array.Empty<int>();

	[Column("blue_points")]
	public int BluePoints { get; set; }
	[Column("red_points")]
	public int RedPoints { get; set; }

	[Column("winner_team")]
	public int? WinnerTeam { get; set; }

	[Column("loser_team")]
	public int? LoserTeam { get; set; }
	[Column("match_type")]
	public MatchType? MatchType { get; set; }

	[InverseProperty("WinRecord")]
	public virtual Match Match { get; set; }
}

public enum MatchType
{
	Team = 1,
	HeadToHead = 2
}
