using Common.Enums;
using Database.Entities.Interfaces;

namespace Database.Entities;

/// <summary>
/// Describes roster information for both teams in a <see cref="Entities.Game"/>
/// </summary>
public class GameRoster : EntityBase, IRoster
{
    /// <summary>
    /// The player ids who are on this roster
    /// </summary>
    public int[] Roster { get; init; } = [];

    /// <summary>
    /// The team of the roster
    /// </summary>
    public Team Team { get; init; }

    /// <summary>
    /// Combined score of the <see cref="Team"/>
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// Id of the <see cref="Entities.Game"/> that the <see cref="GameRoster"/> was generated for
    /// </summary>
    public int GameId { get; init; }

    /// <summary>
    /// The <see cref="Entities.Game"/> that the <see cref="GameRoster"/> was generated for
    /// </summary>
    public Game Game { get; init; } = null!;
}
