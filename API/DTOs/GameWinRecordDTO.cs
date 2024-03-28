using API.Entities;
using AutoMapper;

namespace API.DTOs;

[AutoMap(typeof(GameWinRecord))]
public class GameWinRecordDTO
{
    public int GameId { get; set; }
    public int[] Winners { get; set; } = [];
    public int[] Losers { get; set; } = [];
    public int WinnerTeam { get; set; }
    public int LoserTeam { get; set; }
}
