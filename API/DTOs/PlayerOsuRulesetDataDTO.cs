using Database.Entities;
using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Data for a <see cref="Player"/> in a <see cref="Ruleset"/> obtained from the osu! API and/or osu!Track API
/// </summary>
public class PlayerOsuRulesetDataDTO
{
    /// <summary>
    /// The <see cref="Database.Enums.Ruleset"/> the <see cref="PlayerOsuRulesetData"/> is for
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
    /// Global rank approximately at the time of the <see cref="Player"/>'s first appearance in a <see cref="Match"/>
    /// </summary>
    public int? EarliestGlobalRank { get; set; }

    /// <summary>
    /// Timestamp for when the <see cref="EarliestGlobalRank"/> was recorded
    /// </summary>
    public DateTime? EarliestGlobalRankDate { get; set; }
}
