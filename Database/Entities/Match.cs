using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Interfaces;
using Database.Entities.Processor;
using Database.Enums.Verification;
using Database.Utilities;

namespace Database.Entities;

/// <summary>
/// A match played in a <see cref="Tournament"/>
/// </summary>
[Table("matches")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Match : UpdateableEntityBase, IProcessableEntity, IAdminNotableEntity<MatchAdminNote>,
    IAuditableEntity<MatchAudit>
{
    /// <summary>
    /// osu! id
    /// </summary>
    /// <example>https://osu.ppy.sh/community/matches/[113475484]</example>
    [Column("osu_id")]
    public long OsuId { get; set; }

    /// <summary>
    /// Name of the lobby the match was played in
    /// </summary>
    /// <example>5WC2024: (France) vs (Germany)</example>
    [MaxLength(512)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp for the beginning of the match
    /// </summary>
    [Column("start_time")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Timestamp for the end of the match
    /// </summary>
    [Column("end_time")]
    public DateTime EndTime { get; set; }

    [Column("verification_status")]
    public VerificationStatus VerificationStatus { get; set; }

    [Column("last_processing_date")]
    public DateTime LastProcessingDate { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    [Column("rejection_reason")]
    public MatchRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Warning flags
    /// </summary>
    [Column("warning_flags")]
    public MatchWarningFlags WarningFlags { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    [Column("processing_status")]
    public MatchProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// Id of the <see cref="Entities.Tournament"/> the match was played in
    /// </summary>
    [Column("tournament_id")]
    public int TournamentId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Tournament"/> the match was played in
    /// </summary>
    public Tournament Tournament { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="User"/> that submitted the match
    /// </summary>
    [Column("submitted_by_user_id")]
    public int? SubmittedByUserId { get; set; }

    /// <summary>
    /// The <see cref="User"/> that submitted the match
    /// </summary>
    public User? SubmittedByUser { get; set; }

    /// <summary>
    /// Id of the <see cref="User"/> that verified the match
    /// </summary>
    [Column("verified_by_user_id")]
    public int? VerifiedByUserId { get; set; }

    /// <summary>
    /// The <see cref="User"/> that verified the match
    /// </summary>
    public User? VerifiedByUser { get; set; }

    /// <summary>
    /// The <see cref="MatchWinRecord"/>
    /// </summary>
    public MatchWinRecord? WinRecord { get; set; }

    /// <summary>
    /// A collection of the <see cref="Game"/>s played in the match
    /// </summary>
    public ICollection<Game> Games { get; set; } = new List<Game>();

    /// <summary>
    /// A collection of <see cref="Entities.PlayerMatchStats"/>, one for each <see cref="Player"/> that participated
    /// </summary>
    public ICollection<PlayerMatchStats> PlayerMatchStats { get; set; } = new List<PlayerMatchStats>();

    /// <summary>
    /// A collection of <see cref="RatingAdjustment"/>s, one for each <see cref="Player"/> that participated
    /// </summary>
    public ICollection<RatingAdjustment> PlayerRatingAdjustments { get; set; } = new List<RatingAdjustment>();

    public ICollection<MatchAudit> Audits { get; set; } = new List<MatchAudit>();

    public ICollection<MatchAdminNote> AdminNotes { get; set; } = new List<MatchAdminNote>();

    [NotMapped]
    public int? ActionBlamedOnUserId { get; set; }

    public void ResetAutomationStatuses(bool force)
    {
        var matchUpdate = force || (VerificationStatus != VerificationStatus.Rejected &&
                                    VerificationStatus != VerificationStatus.Verified);

        if (!matchUpdate)
        {
            return;
        }

        VerificationStatus = VerificationStatus.None;
        WarningFlags = MatchWarningFlags.None;
        RejectionReason = MatchRejectionReason.None;
        ProcessingStatus = MatchProcessingStatus.NeedsAutomationChecks;
    }

    public void ConfirmPreVerificationStatus() => VerificationStatus = EnumUtils.ConfirmPreStatus(VerificationStatus);
}
