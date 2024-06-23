using System.ComponentModel.DataAnnotations.Schema;
using Database.Entities.Interfaces;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Describes roster information for both teams in a <see cref="Entities.Game"/>
/// </summary>
[Table("game_win_records")]
public class GameWinRecord : EntityBase, IWinRecord
{
    [Column("winner_roster")]
    public int[] WinnerRoster { get; init; } = [];

    [Column("loser_roster")]
    public int[] LoserRoster { get; init; } = [];

    [Column("winner_team")]
    public Team WinnerTeam { get; init; }

    [Column("loser_team")]
    public Team LoserTeam { get; init; }

    /// <summary>
    /// Combined score of the <see cref="WinnerTeam"/>
    /// </summary>
    [Column("winner_score")]
    public int WinnerScore { get; init; }

    /// <summary>
    /// Combined score of the <see cref="WinnerTeam"/>
    /// </summary>
    [Column("loser_score")]
    public int LoserScore { get; init; }

    /// <summary>
    /// Id of the <see cref="Entities.Game"/> that the <see cref="GameWinRecord"/> was generated for
    /// </summary>
    [Column("game_id")]
    public int GameId { get; init; }

    /// <summary>
    /// The <see cref="Entities.Game"/> that the <see cref="GameWinRecord"/> was generated for
    /// </summary>
    public Game Game { get; init; } = null!;
}
