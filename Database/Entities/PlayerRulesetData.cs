using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Data
/// </summary>
public class PlayerRulesetData
{
    /// <summary>
    /// The <see cref="Enums.Ruleset"/> the <see cref="PlayerRulesetData"/> is for
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Performance points
    /// </summary>
    public double Pp { get; set; }

    /// <summary>
    /// Last recorded global rank
    /// </summary>
    public int GlobalRank { get; set; }

    /// <summary>
    /// Earliest known global rank at the time of the <see cref="Player"/>'s first appearance in a <see cref="Match"/>
    /// </summary>
    public int? EarliestGlobalRank { get; set; }

    /// <summary>
    /// Timestamp of access for the <see cref="EarliestGlobalRank"/>
    /// </summary>
    public DateTime? EarliestGlobalRankDate { get; set; }
}
