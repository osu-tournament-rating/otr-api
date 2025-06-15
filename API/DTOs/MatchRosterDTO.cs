using System.ComponentModel.DataAnnotations;
using Common.Enums;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents roster information for teams in a match
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MatchRosterDTO
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Player IDs for this roster
    /// </summary>
    public int[] Roster { get; set; } = [];

    /// <summary>
    /// The team designation
    /// </summary>
    [EnumDataType(typeof(Team))]
    public Team Team { get; set; }

    /// <summary>
    /// The total score for this roster
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Id of the match this roster belongs to
    /// </summary>
    public int MatchId { get; set; }
}
