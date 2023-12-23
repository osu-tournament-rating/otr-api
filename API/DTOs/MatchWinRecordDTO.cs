namespace API.DTOs;

/// <summary>
/// Provided by the data processor, this DTO represents a match-level pairing, similar
/// to <see cref="GameWinRecordDTO"/>
/// </summary>
public class MatchWinRecordDTO
{
	public int MatchId { get; set; }
	public int[] Team1 { get; set; } = Array.Empty<int>();
	public int[] Team2 { get; set; } = Array.Empty<int>();
	public int Team1Points { get; set; }
	public int Team2Points { get; set; }
	public int? WinnerTeam { get; set; }
	public int? LoserTeam { get; set; }
}