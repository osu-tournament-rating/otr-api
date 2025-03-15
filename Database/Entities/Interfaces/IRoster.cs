using Common.Enums;

namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces a record of roster information for a <see cref="Match"/> or <see cref="Game"/>
/// </summary>
public interface IRoster
{
    /// <summary>
    /// List of <see cref="Player"/> ids on the team
    /// </summary>
    public int[] Roster { get; }

    /// <summary>
    /// The <see cref="Common.Enums.Team"/> the roster played for
    /// </summary>
    public Team Team { get; }

    /// <summary>
    /// Total score of the roster
    /// </summary>
    public int Score { get; }
}
