using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Common.Enums.Verification;
using Common.Utilities.Extensions;
using Database.Entities.Interfaces;
using Database.Utilities;
using LinqKit;

namespace Database.Entities;

/// <summary>
/// An osu! tournament
/// </summary>
public class Tournament : UpdateableEntityBase, IProcessableEntity, IAdminNotableEntity<TournamentAdminNote>
{
    private string _name = string.Empty;

    /// <summary>
    /// Name
    /// </summary>
    [MaxLength(512)]
    public string Name
    {
        get => string.IsNullOrEmpty(_name) ? $"Tournament {Id}" : _name;
        set => _name = value;
    }

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
    /// A collection of <see cref="TournamentAdminNote"/>s for the tournament
    /// </summary>
    public ICollection<TournamentAdminNote> AdminNotes { get; set; } = [];

    /// <summary>
    /// Collection of <see cref="TournamentAudit"/> records for the <see cref="Tournament"/>
    /// </summary>
    public ICollection<TournamentAudit> Audits { get; set; } = new List<TournamentAudit>();

    /// <summary>
    /// A collection of <see cref="Beatmap"/>s pooled in the tournament
    /// </summary>
    public ICollection<Beatmap> PooledBeatmaps { get; set; } = [];

    public void ResetAutomationStatuses(bool force)
    {
        bool update = force || (VerificationStatus != VerificationStatus.Rejected &&
                               VerificationStatus != VerificationStatus.Verified);

        if (!update)
        {
            return;
        }

        VerificationStatus = VerificationStatus.None;
        RejectionReason = TournamentRejectionReason.None;
        ProcessingStatus = TournamentProcessingStatus.NeedsAutomationChecks;
    }

    /// <summary>
    /// Confirms pre-verification statuses for this tournament and optionally all its child entities,
    /// applying cascading rejection logic and clearing warning flags for verified entities
    /// </summary>
    /// <param name="verifierUserId">The ID of the user performing the verification</param>
    /// <param name="includeChildren">Whether to also confirm pre-verification statuses for child entities</param>
    public void ConfirmPreVerification(int verifierUserId, bool includeChildren = true)
    {
        VerificationStatus = VerificationStatus.ConfirmPreStatus();
        VerifiedByUserId = verifierUserId;

        if (!includeChildren)
        {
            return;
        }

        if (VerificationStatus == VerificationStatus.Rejected)
        {
            RejectAllChildren();
        }
        else
        {
            Matches.ForEach(match => match.ConfirmPreVerification(verifierUserId, includeChildren));
        }
    }

    /// <summary>
    /// Rejects all child entities in this tournament
    /// </summary>
    public void RejectAllChildren()
    {
        foreach (Match match in Matches)
        {
            match.VerificationStatus = VerificationStatus.Rejected;
            match.RejectionReason |= MatchRejectionReason.RejectedTournament;
            match.ProcessingStatus = MatchProcessingStatus.Done;

            match.RejectAllChildren();
        }
    }
}
