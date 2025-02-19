using System.ComponentModel.DataAnnotations.Schema;
using Database.Entities.Interfaces;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Describes roster information for both teams in a <see cref="Entities.Game"/>
/// </summary>
public class GameWinRecord : EntityBase, IWinRecord
{
    public int[] WinnerRoster { get; init; } = [];

    public int[] LoserRoster { get; init; } = [];

    public Team WinnerTeam { get; init; }

    public Team LoserTeam { get; init; }

    /// <summary>
    /// Combined score of the <see cref="WinnerTeam"/>
    /// </summary>
    public int WinnerScore { get; init; }

    /// <summary>
    /// Combined score of the <see cref="WinnerTeam"/>
    /// </summary>
    public int LoserScore { get; init; }

    /// <summary>
    /// Id of the <see cref="Entities.Game"/> that the <see cref="GameWinRecord"/> was generated for
    /// </summary>
    public int GameId { get; init; }

    /// <summary>
    /// The <see cref="Entities.Game"/> that the <see cref="GameWinRecord"/> was generated for
    /// </summary>
    public Game Game { get; init; } = null!;
}
