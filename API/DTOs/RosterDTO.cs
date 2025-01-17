using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Represents a roster of players in a game or match
/// </summary>
public class RosterDTO
{
    /// <summary>
    /// Team the players were on
    /// </summary>
    public Team Team { get; init; }

    /// <summary>
    /// Ids of all players on the team
    /// </summary>
    public int[] Roster { get; init; } = [];

    /// <summary>
    /// Denotes if the team won or lost
    /// </summary>
    public bool Won { get; init; }

    /// <summary>
    /// Total score
    /// </summary>
    public int Score { get; init; }
}
