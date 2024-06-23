using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;
using Database.Enums.Verification;

namespace Database.Entities;

/// <summary>
/// An osu! tournament
/// </summary>
[Table("tournaments")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Tournament : UpdateableEntityBase
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
    /// Verification status
    /// </summary>
    [Column("verification_status")]
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    [Column("processing_status")]
    public TournamentProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    [Column("rejection_reason")]
    public TournamentRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Id of the user that submitted the tournament
    /// </summary>
    [Column("submitted_by_user_id")]
    public int SubmittedByUserId { get; set; }

    /// <summary>
    /// The <see cref="User"/> that submitted the tournament
    /// </summary>
    public User? SubmittedByUser { get; set; }

    /// <summary>
    /// Id of the user that verified the tournament
    /// </summary>
    [Column("verified_by_user_id")]
    public int? VerifiedByUserId { get; set; }

    /// <summary>
    /// The <see cref="User"/> that verified the tournament
    /// </summary>
    public User? VerifiedByUser { get; set; }

    /// <summary>
    /// A collection of <see cref="Match"/>es played in the tournament
    /// </summary>
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
