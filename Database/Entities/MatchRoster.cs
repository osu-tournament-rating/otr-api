using Database.Entities.Interfaces;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Describes roster information for both teams in a <see cref="Entities.Match"/>
/// </summary>
public class MatchRoster : EntityBase, IRoster
{
    public int[] Roster { get; init; } = [];

    public Team Team { get; init; }

    /// <summary>
    /// Number of points earned by the <see cref="Team"/> during the match
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// Id of the <see cref="Entities.Match"/> the <see cref="MatchRoster"/> was generated for
    /// </summary>
    public int MatchId { get; init; }

    /// <summary>
    /// The <see cref="Entities.Match"/> the <see cref="MatchRoster"/> was generated for
    /// </summary>
    public Match Match { get; init; } = null!;
}
