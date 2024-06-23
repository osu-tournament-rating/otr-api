namespace API.DTOs;

public class GameWinRecordDTO
{
    public int GameId { get; set; }
    public int[] WinnerRoster { get; set; } = [];
    public int[] LoserRoster { get; set; } = [];
    public int WinnerTeam { get; set; }
    public int LoserTeam { get; set; }
}
