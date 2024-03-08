namespace API.DTOs;

public class GameWinRecordDTO
{
    public int GameId { get; set; }
    public int[] Winners { get; set; } = [];
    public int[] Losers { get; set; } = [];
    public int WinnerTeam { get; set; }
    public int LoserTeam { get; set; }
}
