using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Interfaces;
using Database.Enums;
using Database.Enums.Verification;
using Database.Utilities;

namespace Database.Entities;

/// <summary>
/// An osu! tournament
/// </summary>
[Table("tournaments")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Tournament : UpdateableEntityBase, IProcessableEntity, IAdminNotableEntity<TournamentAdminNote>,
    IAuditableEntity<TournamentAudit>
{
    /// <summary>
    /// Name
    /// </summary>
    [MaxLength(512)]
    [Column("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Abbreviation
    /// </summary>
    [MaxLength(32)]
    [Column("abbreviation")]
    public string Abbreviation { get; set; } = null!;

    /// <summary>
    /// Link to the osu! forum post
    /// </summary>
    [MaxLength(255)]
    [Column("forum_url")]
    public string ForumUrl { get; set; } = null!;

    /// <summary>
    /// Lower bound of the rank range
    /// </summary>
    [Column("rank_range_lower_bound")]
    public int RankRangeLowerBound { get; set; }

    /// <summary>
    /// The <see cref="Ruleset"/> the tournament was played in
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Expected in-match team size
    /// </summary>
    [Column("lobby_size")]
    public int LobbySize { get; set; }

    [Column("verification_status")] public VerificationStatus VerificationStatus { get; set; }

    [AuditIgnore]
    [Column("last_processing_date")]
    public DateTime LastProcessingDate { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    [Column("rejection_reason")]
    public TournamentRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    [Column("processing_status")]
    public TournamentProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// Id of the <see cref="User"/> that submitted the tournament
    /// </summary>
    [Column("submitted_by_user_id")]
    public int? SubmittedByUserId { get; set; }

    /// <summary>
    /// The <see cref="User"/> that submitted the tournament
    /// </summary>
    public User? SubmittedByUser { get; set; }

    /// <summary>
    /// Id of the <see cref="User"/> that verified the tournament
    /// </summary>
    [Column("verified_by_user_id")]
    public int? VerifiedByUserId { get; set; }

    /// <summary>
    /// The <see cref="User"/> that verified the tournament
    /// </summary>
    public User? VerifiedByUser { get; set; }

    /// <summary>
    /// The start date of the first <see cref="Match"/> played in the tournament
    /// </summary>
    [Column("start_time")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// The end date of the last <see cref="Match"/> played in the tournament
    /// </summary>
    [Column("end_time")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// A collection of <see cref="Match"/>es played in the tournament
    /// </summary>
    public ICollection<Match> Matches { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="Entities.PlayerTournamentStats"/>, one for each <see cref="Player"/> that participated
    /// </summary>
    public ICollection<PlayerTournamentStats> PlayerTournamentStats { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="TournamentAudit"/>s which are used to track the changes
    /// to this entity over time
    /// </summary>
    public ICollection<TournamentAudit> Audits { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="TournamentAdminNote"/>s for the tournament
    /// </summary>
    public ICollection<TournamentAdminNote> AdminNotes { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="Beatmap"/>s pooled in the tournament
    /// </summary>
    public ICollection<Beatmap> PooledBeatmaps { get; set; } = [];

    [NotMapped] public int? ActionBlamedOnUserId { get; set; }

    public void ResetAutomationStatuses(bool force)
    {
        var update = force || (VerificationStatus != VerificationStatus.Rejected &&
                               VerificationStatus != VerificationStatus.Verified);

        if (!update)
        {
            return;
        }

        VerificationStatus = VerificationStatus.None;
        RejectionReason = TournamentRejectionReason.None;
        ProcessingStatus = TournamentProcessingStatus.NeedsAutomationChecks;
    }

    public void ConfirmPreVerificationStatus() => VerificationStatus = EnumUtils.ConfirmPreStatus(VerificationStatus);
}
