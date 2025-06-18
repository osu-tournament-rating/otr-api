using Common.Enums;
using Database.Entities;
using JetBrains.Annotations;

namespace Database.Utilities;

/// <summary>
/// A record of who won and lost a match
/// </summary>
/// <remarks>
/// Generated from MatchRoster data
/// </remarks>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MatchWinRecord
{
    /// <summary>
    /// Creates a MatchWinRecord from a collection of MatchRosters
    /// </summary>
    /// <param name="matchId">The ID of the match</param>
    /// <param name="rosters">The collection of rosters for the match</param>
    public MatchWinRecord(int matchId, ICollection<MatchRoster> rosters)
    {
        if (rosters.Count < 2)
        {
            throw new ArgumentException("At least 2 rosters are required to create a MatchWinRecord", nameof(rosters));
        }

        var sortedRosters = rosters.OrderByDescending(r => r.Score).ToList();
        MatchRoster first = sortedRosters[0];
        MatchRoster second = sortedRosters[1];

        MatchId = matchId;

        if (first.Score == second.Score)
        {
            IsTied = true;
            WinnerRoster = null;
            LoserRoster = null;
            WinnerPoints = first.Score; // Both teams have same score
            LoserPoints = second.Score;
            WinnerTeam = null;
            LoserTeam = null;
        }
        else
        {
            // Normal win/loss scenario
            IsTied = false;
            WinnerRoster = first.Roster;
            LoserRoster = second.Roster;
            WinnerPoints = first.Score;
            LoserPoints = second.Score;
            WinnerTeam = (int?)first.Team;
            LoserTeam = (int?)second.Team;
        }
    }

    /// <summary>
    /// The id of the match
    /// </summary>
    public int MatchId { get; }

    /// <summary>
    /// Indicates whether the match ended in a tie
    /// </summary>
    public bool IsTied { get; }

    /// <summary>
    /// The ids of each player on the losing team. Null if tied.
    /// </summary>
    public int[]? LoserRoster { get; }

    /// <summary>
    /// The ids of each player on the winning team. Null if tied.
    /// </summary>
    public int[]? WinnerRoster { get; }

    /// <summary>
    /// The number of points the losing team earned
    /// </summary>
    public int LoserPoints { get; }

    /// <summary>
    /// The number of points the winning team earned
    /// </summary>
    public int WinnerPoints { get; }

    /// <summary>
    /// The winning team (see <see cref="Team"/>). Null if HeadToHead or tied.
    /// </summary>
    public int? WinnerTeam { get; }

    /// <summary>
    /// The losing team (see <see cref="Team"/>). Null if HeadToHead or tied.
    /// </summary>
    public int? LoserTeam { get; }
}
