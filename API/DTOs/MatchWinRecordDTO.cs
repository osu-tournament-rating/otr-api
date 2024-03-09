namespace API.DTOs;

/// <summary>
/// Provided by the data processor, this DTO represents a match-level pairing, similar
/// to <see cref="GameWinRecordDTO"/>
/// </summary>
public class MatchWinRecordDTO
{
    public int MatchId { get; set; }
    public int[] TeamBlue { get; set; } = [];
    public int[] TeamRed { get; set; } = [];
    public int BluePoints { get; set; }
    public int RedPoints { get; set; }
    public int? WinnerTeam { get; set; }
    public int? LoserTeam { get; set; }
    public int? MatchType { get; set; }
}
