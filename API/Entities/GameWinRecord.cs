using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

/// <summary>
///  A record of who won a game
/// </summary>
[Table("game_win_records")]
public sealed class GameWinRecord
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("game_id")]
    public int GameId { get; set; }

    [Column("winners")]
    public int[] Winners { get; set; } = [];

    [Column("losers")]
    public int[] Losers { get; set; } = [];

    [Column("winner_team")]
    public int WinnerTeam { get; set; }

    [Column("loser_team")]
    public int LoserTeam { get; set; }

    [InverseProperty("WinRecord")]
    public Game Game { get; set; } = null!;
}
