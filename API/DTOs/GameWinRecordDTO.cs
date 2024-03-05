namespace API.DTOs;

public class GameWinRecordDTO
{
    public int GameId { get; set; }
    public int[] Winners { get; set; } = Array.Empty<int>();
    public int[] Losers { get; set; } = Array.Empty<int>();
    public int WinnerTeam { get; set; }
    public int LoserTeam { get; set; }
}
