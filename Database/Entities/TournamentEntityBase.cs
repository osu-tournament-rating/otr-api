using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace Database.Entities;

/// <summary>
/// Base entity for tournaments
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[Index(nameof(Name), nameof(Abbreviation), IsUnique = true)]
public abstract class TournamentEntityBase : UpdateableEntityBase
{
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

    /// <summary>
    /// Name of the tournament
    /// </summary>
    [Column("name")]
    [Length(minimumLength: 1, maximumLength: 512)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Abbreviation of the tournament
    /// </summary>
    [Column("abbreviation")]
    [Length(minimumLength: 1, maximumLength: 32)]
    public string Abbreviation { get; set; } = null!;

    /// <summary>
    /// Link to the forum post for the tournament
    /// </summary>
    [Column("forum_url")]
    [Length(minimumLength: 1, maximumLength: 255)]
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
    /// The <see cref="User"/> that submitted the tournament
    /// </summary>
    [ForeignKey(nameof(User.SubmittedTournaments))]
    public User SubmittedBy { get; set; } = null!;

    /// <summary>
    /// The <see cref="User"/> that verified the tournament
    /// </summary>
    [ForeignKey(nameof(User.VerifiedTournaments))]
    public User VerifiedBy { get; set; } = null!;
}
