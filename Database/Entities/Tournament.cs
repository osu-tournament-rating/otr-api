using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities.Interfaces;
using Database.Utilities;

namespace Database.Entities;

/// <summary>
/// An osu! tournament
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Tournament : UpdateableEntityBase, IProcessableEntity, IAdminNotableEntity<TournamentAdminNote>,
    IAuditableEntity<TournamentAudit>
{
    /// <summary>
    /// Name
    /// </summary>
    [MaxLength(512)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Abbreviation
    /// </summary>
    [MaxLength(32)]
    public string Abbreviation { get; set; } = null!;

    /// <summary>
    /// Link to the osu! forum post
    /// </summary>
    [MaxLength(255)]
    public string ForumUrl { get; set; } = null!;

    /// <summary>
    /// Lower bound of the rank range
    /// </summary>
    public int RankRangeLowerBound { get; set; }

    /// <summary>
    /// The <see cref="Ruleset"/> the tournament was played in
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Expected in-match team size
    /// </summary>
    public int LobbySize { get; set; }

    public VerificationStatus VerificationStatus { get; set; }

    [AuditIgnore]
    public DateTime LastProcessingDate { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    public TournamentRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    public TournamentProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// Id of the <see cref="User"/> that submitted the tournament
    /// </summary>
    public int? SubmittedByUserId { get; set; }

    /// <summary>
    /// The <see cref="User"/> that submitted the tournament
    /// </summary>
    public User? SubmittedByUser { get; set; }

    /// <summary>
    /// Id of the <see cref="User"/> that verified the tournament
    /// </summary>
    public int? VerifiedByUserId { get; set; }

    /// <summary>
    /// The <see cref="User"/> that verified the tournament
    /// </summary>
    public User? VerifiedByUser { get; set; }

    /// <summary>
    /// The start date of the first <see cref="Match"/> played in the tournament
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// The end date of the last <see cref="Match"/> played in the tournament
    /// </summary>
    public DateTime? EndTime { get; set; }

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
