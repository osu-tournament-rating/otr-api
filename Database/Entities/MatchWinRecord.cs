using System.ComponentModel.DataAnnotations.Schema;
using Database.Entities.Interfaces;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Describes roster information for both teams in a <see cref="Entities.Match"/>
/// </summary>
[Table("match_win_records")]
public class MatchWinRecord : EntityBase, IWinRecord
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
    /// Number of <see cref="Game"/>s won by the <see cref="WinnerTeam"/>
    /// </summary>
    [Column("winner_score")]
    public int WinnerScore { get; init; }

    /// <summary>
    /// Number of <see cref="Game"/>s won by the <see cref="WinnerTeam"/>
    /// </summary>
    [Column("loser_score")]
    public int LoserScore { get; init; }

    /// <summary>
    /// Id of the <see cref="Entities.Match"/> the <see cref="MatchWinRecord"/> was generated for
    /// </summary>
    [Column("match_id")]
    public int MatchId { get; init; }

    /// <summary>
    /// The <see cref="Entities.Match"/> the <see cref="MatchWinRecord"/> was generated for
    /// </summary>
    public Match Match { get; init; } = null!;
}
