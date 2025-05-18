using Common.Enums;

namespace Database.Entities;

/// <summary>
/// Data for a <see cref="Entities.Player"/> in a <see cref="Common.Enums.Ruleset"/>
/// obtained from the osu! API and/or osu!Track API
/// </summary>
public class PlayerOsuRulesetData : UpdateableEntityBase
{
    /// <summary>
    /// The <see cref="Common.Enums.Ruleset"/> the <see cref="PlayerOsuRulesetData"/> is for
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

    /// <summary>
    /// Id of the <see cref="Entities.Player"/> that owns the osu ruleset data
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Player"/> that owns the osu ruleset data
    /// </summary>
    public Player Player { get; set; } = null!;
}
