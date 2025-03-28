using Common.Enums;

namespace API.DTOs;

/// <summary>
/// A record of who won and lost a match
/// </summary>
/// <remarks>
/// Generated by the o!TR Processor
/// </remarks>
public class MatchWinRecordDTO
{
    /// <summary>
    /// The id of the match
    /// </summary>
    public int MatchId { get; set; }
    /// <summary>
    /// The ids of each player on the losing team
    /// </summary>
    public int[] LoserRoster { get; set; } = [];
    /// <summary>
    /// The ids of each player on the winning team
    /// </summary>
    public int[] WinnerRoster { get; set; } = [];
    /// <summary>
    /// The number of points the losing team earned
    /// </summary>
    public int LoserPoints { get; set; }
    /// <summary>
    /// The number of points the winning team earned
    /// <summary>
    public int WinnerPoints { get; set; }
    /// <summary>
    /// The winning team (see <see cref="Team"/>). Null if HeadToHead.
    /// </summary>
    public int? WinnerTeam { get; set; }
    /// <summary>
    /// The losing team (see <see cref="Team"/>). Null if HeadToHead.
    /// </summary>
    public int? LoserTeam { get; set; }
    /// <summary>
    /// The type of match (see <see cref="Database.MatchType"/>). Null if not able to be determined.
    /// </summary>
    public int? MatchType { get; set; }
}
