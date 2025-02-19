using System.ComponentModel.DataAnnotations.Schema;
using Database.Entities.Interfaces;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Describes roster information for both teams in a <see cref="Entities.Match"/>
/// </summary>
public class MatchWinRecord : EntityBase, IWinRecord
{
    public int[] WinnerRoster { get; init; } = [];

    public int[] LoserRoster { get; init; } = [];

    public Team WinnerTeam { get; init; }

    public Team LoserTeam { get; init; }

    /// <summary>
    /// Number of <see cref="Game"/>s won by the <see cref="WinnerTeam"/>
    /// </summary>
    public int WinnerScore { get; init; }

    /// <summary>
    /// Number of <see cref="Game"/>s won by the <see cref="WinnerTeam"/>
    /// </summary>
    public int LoserScore { get; init; }

    /// <summary>
    /// Id of the <see cref="Entities.Match"/> the <see cref="MatchWinRecord"/> was generated for
    /// </summary>
    public int MatchId { get; init; }

    /// <summary>
    /// The <see cref="Entities.Match"/> the <see cref="MatchWinRecord"/> was generated for
    /// </summary>
    public Match Match { get; init; } = null!;
}
