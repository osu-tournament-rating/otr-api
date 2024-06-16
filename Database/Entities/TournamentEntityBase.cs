using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Base entity for tournaments
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public abstract class TournamentEntityBase : UpdateableEntityBase
{
    /// <summary>
    /// Name of the tournament
    /// </summary>
    [MaxLength(512)]
    [Column("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Abbreviation of the tournament
    /// </summary>
    [MaxLength(32)]
    [Column("abbreviation")]
    public string Abbreviation { get; set; } = null!;

    /// <summary>
    /// Link to the forum post for the tournament
    /// </summary>
    [MaxLength(255)]
    [Column("forum_url")]
    public string ForumUrl { get; set; } = null!;

    /// <summary>
    /// Lower bound of the rank range for the tournament
    /// </summary>
    [Column("rank_range_lower_bound")]
    public int RankRangeLowerBound { get; set; }

    /// <summary>
    /// Ruleset the tournament was played in
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Expected in-game team size for the tournament
    /// </summary>
    [Column("team_size")]
    public int TeamSize { get; set; }

    /// <summary>
    /// Id of the user that submitted the tournament
    /// </summary>
    [Column("submitted_by_user_id")]
    public int SubmittedByUserId { get; set; }

    /// <summary>
    /// Id of the user that verified the tournament
    /// </summary>
    [Column("verified_by_user_id")]
    public int? VerifiedByUserId { get; set; }
}
