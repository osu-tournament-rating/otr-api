namespace API.DTOs;

/// <summary>
/// Provided by the data processor, this DTO represents a match-level pairing, similar
/// to <see cref="GameWinRecordDTO"/>
/// </summary>
public class MatchWinRecordDTO
{
    public int MatchId { get; set; }
    public int[] LoserRoster { get; set; } = [];
    public int[] WinnerRoster { get; set; } = [];
    public int LoserPoints { get; set; }
    public int WinnerPoints { get; set; }
    public int? WinnerTeam { get; set; }
    public int? LoserTeam { get; set; }
    public int? MatchType { get; set; }
}
