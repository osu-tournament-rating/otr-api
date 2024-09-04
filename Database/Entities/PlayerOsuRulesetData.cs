using System.ComponentModel.DataAnnotations.Schema;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Data for a <see cref="Entities.Player"/> in a <see cref="Enums.Ruleset"/>
/// obtained from the osu! API and/or osu!Track API
/// </summary>
[Table("player_osu_ruleset_data")]
public class PlayerOsuRulesetData : UpdateableEntityBase
{
    /// <summary>
    /// The <see cref="Enums.Ruleset"/> the <see cref="PlayerOsuRulesetData"/> is for
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Performance points
    /// </summary>
    [Column("pp")]
    public double Pp { get; set; }

    /// <summary>
    /// Last recorded global rank
    /// </summary>
    [Column("global_rank")]
    public int GlobalRank { get; set; }

    /// <summary>
    /// Global rank approximately at the time of the <see cref="Player"/>'s first appearance in a <see cref="Match"/>
    /// </summary>
    [Column("earliest_global_rank")]
    public int? EarliestGlobalRank { get; set; }

    /// <summary>
    /// Timestamp for when the <see cref="EarliestGlobalRank"/> was recorded
    /// </summary>
    [Column("earliest_global_rank_date")]
    public DateTime? EarliestGlobalRankDate { get; set; }

    /// <summary>
    /// Id of the <see cref="Entities.Player"/> that owns the osu ruleset data
    /// </summary>
    [Column("player_id")]
    public int PlayerId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Player"/> that owns the osu ruleset data
    /// </summary>
    public Player Player { get; set; } = null!;
}
