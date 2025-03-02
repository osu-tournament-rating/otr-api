using Common.Enums.Enums;

namespace API.DTOs;

/// <summary>
/// Represents aggregate statistics and roster for both teams in a game
/// </summary>
public class GameRosterDTO
{
    /// <summary>
    /// Id of the game
    /// </summary>
    public int GameId { get; init; }

    /// <summary>
    /// Winning team
    /// </summary>
    public Team Team { get; init; }

    /// <summary>
    /// Combined score of the losing team
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// Ids of all players on the losing team
    /// </summary>
    public int[] Roster { get; init; } = [];
}
