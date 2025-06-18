using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Common.Enums.Verification;
using Common.Utilities.Extensions;
using Database.Entities.Interfaces;
using Database.Entities.Processor;
using Database.Utilities;
using LinqKit;

namespace Database.Entities;

/// <summary>
/// A match played in a <see cref="Tournament"/>
/// </summary>
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Match : UpdateableEntityBase, IProcessableEntity, IAdminNotableEntity<MatchAdminNote>
{
    private string _name = string.Empty;

    /// <summary>
    /// osu! id
    /// </summary>
    /// <example>https://osu.ppy.sh/community/matches/[113475484]</example>
    public long OsuId { get; set; }

    /// <summary>
    /// Name of the lobby the match was played in
    /// </summary>
    /// <example>5WC2024: (France) vs (Germany)</example>
    [MaxLength(512)]
    public string Name
    {
        get => string.IsNullOrEmpty(_name) ? $"Match {Id}" : _name;
        set => _name = value;
    }

    /// <summary>
    /// Timestamp for the beginning of the match
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// Timestamp for the end of the match
    /// </summary>
    public DateTime? EndTime { get; set; }

    public VerificationStatus VerificationStatus { get; set; }

    [AuditIgnore]
    public DateTime LastProcessingDate { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    public MatchRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Warning flags
    /// </summary>
    public MatchWarningFlags WarningFlags { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    public MatchProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// Id of the <see cref="Entities.Tournament"/> the match was played in
    /// </summary>
    public int TournamentId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Tournament"/> the match was played in
    /// </summary>
    public Tournament Tournament { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="User"/> that submitted the match
    /// </summary>
    public int? SubmittedByUserId { get; set; }

    /// <summary>
    /// The <see cref="User"/> that submitted the match
    /// </summary>
    public User? SubmittedByUser { get; set; }

    /// <summary>
    /// Id of the <see cref="User"/> that verified the match
    /// </summary>
    public int? VerifiedByUserId { get; set; }

    /// <summary>
    /// The <see cref="User"/> that verified the match
    /// </summary>
    public User? VerifiedByUser { get; set; }

    /// <summary>
    /// The <see cref="MatchRoster"/>
    /// </summary>
    public ICollection<MatchRoster> Rosters { get; set; } = [];

    /// <summary>
    /// A collection of the <see cref="Game"/>s played in the match
    /// </summary>
    public ICollection<Game> Games { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="Entities.PlayerMatchStats"/>, one for each <see cref="Player"/> that participated
    /// </summary>
    public ICollection<PlayerMatchStats> PlayerMatchStats { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="RatingAdjustment"/>s, one for each <see cref="Player"/> that participated
    /// </summary>
    public ICollection<RatingAdjustment> PlayerRatingAdjustments { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="MatchAdminNote"/>s for the <see cref="Match"/>
    /// </summary>
    public ICollection<MatchAdminNote> AdminNotes { get; set; } = [];

    /// <summary>
    /// Collection of <see cref="MatchAudit"/> records for the <see cref="Match"/>
    /// </summary>
    public ICollection<MatchAudit> Audits { get; set; } = new List<MatchAudit>();

    /// <summary>
    /// Win record for the match based on the Rosters
    /// </summary>
    /// <remarks>
    /// This is not stored in the database but computed from the Rosters collection.
    /// Returns null only if there are fewer than 2 rosters.
    /// </remarks>
    [NotMapped]
    public MatchWinRecord? WinRecord
    {
        get
        {
            try
            {
                return new MatchWinRecord(Id, Rosters);
            }
            catch (ArgumentException)
            {
                // No valid win record can be created (not enough rosters)
                return null;
            }
        }
    }

    public void ResetAutomationStatuses(bool force)
    {
        bool matchUpdate = force || (VerificationStatus != VerificationStatus.Rejected &&
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

    /// <summary>
    /// Confirms pre-verification statuses for this match and optionally all its child entities,
    /// applying cascading rejection logic and clearing warning flags for verified entities
    /// </summary>
    /// <param name="verifierUserId">The ID of the user performing the verification</param>
    /// <param name="includeChildren">Whether to also confirm pre-verification statuses for child entities</param>
    public void ConfirmPreVerification(int verifierUserId, bool includeChildren = true)
    {
        VerificationStatus = VerificationStatus.ConfirmPreStatus();
        VerifiedByUserId = verifierUserId;

        if (VerificationStatus == VerificationStatus.Verified)
        {
            WarningFlags = MatchWarningFlags.None;
        }

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
            Games.ForEach(game => game.ConfirmPreVerification(includeChildren));
        }
    }

    /// <summary>
    /// Rejects all child entities in this match
    /// </summary>
    public void RejectAllChildren()
    {
        foreach (Game game in Games)
        {
            game.VerificationStatus = VerificationStatus.Rejected;
            game.RejectionReason |= GameRejectionReason.RejectedMatch;
            game.ProcessingStatus = GameProcessingStatus.Done;

            game.RejectAllChildren();
        }
    }
}
