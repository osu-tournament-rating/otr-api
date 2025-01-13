using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Represents aggregate statistics and roster for both teams in a game
/// </summary>
public class GameWinRecordDTO
{
    /// <summary>
    /// Id of the game
    /// </summary>
    public int GameId { get; init; }

    /// <summary>
    /// Winning team
    /// </summary>
    public Team WinnerTeam { get; init; }

    /// <summary>
    /// Losing team
    /// </summary>
    public Team LoserTeam { get; init; }

    /// <summary>
    /// Combined score of the winning team
    /// </summary>
    public int WinnerScore { get; init; }

    /// <summary>
    /// Combined score of the losing team
    /// </summary>
    public int LoserScore { get; init; }

    /// <summary>
    /// Ids of all players on the winning team
    /// </summary>
    public int[] WinnerRoster { get; init; } = [];

    /// <summary>
    /// Ids of all players on the losing team
    /// </summary>
    public int[] LoserRoster { get; init; } = [];
}
