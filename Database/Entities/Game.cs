using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Common.Enums.Verification;
using Common.Utilities.Extensions;
using Database.Entities.Interfaces;
using Database.Utilities;
using LinqKit;

namespace Database.Entities;

/// <summary>
/// A game played in a <see cref="Match"/>
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Game : UpdateableEntityBase, IProcessableEntity, IAdminNotableEntity<GameAdminNote>
{
    /// <summary>
    /// osu! id
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// The <see cref="Common.Enums.Ruleset"/> the game was played in
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// The <see cref="Common.Enums.ScoringType"/> used
    /// </summary>
    public ScoringType ScoringType { get; set; }

    /// <summary>
    /// The <see cref="Common.Enums.TeamType"/> used
    /// </summary>
    public TeamType TeamType { get; set; }

    /// <summary>
    /// The <see cref="Common.Enums.Mods"/> enabled for the game
    /// </summary>
    /// <remarks>
    /// Mods set on the game level are "forced" on all scores
    /// </remarks>
    public Mods Mods { get; set; }

    /// <summary>
    /// Timestamp for the beginning of the game
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Timestamp for the end of the game
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Verification status
    /// </summary>
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    public GameRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Warning flags
    /// </summary>
    public GameWarningFlags WarningFlags { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    public GameProcessingStatus ProcessingStatus { get; set; }

    [AuditIgnore]
    public DateTime LastProcessingDate { get; set; }

    /// <summary>
    /// Id of the <see cref="Entities.Match"/> that the game was played in
    /// </summary>
    public int MatchId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Match"/> the game was played in
    /// </summary>
    public Match Match { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="Entities.Beatmap"/> played during the game
    /// </summary>
    public int? BeatmapId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Beatmap"/> played during the game
    /// </summary>
    public Beatmap? Beatmap { get; set; }

    /// <summary>
    /// The rosters which participated in the game
    /// </summary>
    public ICollection<GameRoster> Rosters { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="GameScore"/>s set in the <see cref="Game"/>
    /// </summary>
    public ICollection<GameScore> Scores { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="GameAdminNote"/>s for the <see cref="Game"/>
    /// </summary>
    public ICollection<GameAdminNote> AdminNotes { get; set; } = [];

    /// <summary>
    /// Collection of <see cref="GameAudit"/> records for the <see cref="Game"/>
    /// </summary>
    public ICollection<GameAudit> Audits { get; set; } = new List<GameAudit>();

    /// <summary>
    /// Denotes if the mod setting was "free mod"
    /// </summary>
    [NotMapped]
    public bool IsFreeMod =>
        // No forced mod
        Mods is Mods.None
        // The forced mod is only HT or DT
        || (Mods is Mods.HalfTime or Mods.DoubleTime
            // Any scores include HT or DT, but are not only HT or DT
            && Scores.Any(s => s.Mods.HasFlag(Mods) && s.Mods != Mods));

    /// <summary>
    /// Resets the automation statuses for the <see cref="Game"/>
    /// </summary>
    /// <param name="force">Whether to extend this reset to verified and rejected data</param>
    /// <remarks>
    /// Child entities are not affected
    /// </remarks>
    public void ResetAutomationStatuses(bool force)
    {
        bool gameUpdate = force || (VerificationStatus != VerificationStatus.Rejected &&
                                   VerificationStatus != VerificationStatus.Verified);

        if (!gameUpdate)
        {
            return;
        }

        VerificationStatus = VerificationStatus.None;
        WarningFlags = GameWarningFlags.None;
        RejectionReason = GameRejectionReason.None;
        ProcessingStatus = GameProcessingStatus.NeedsAutomationChecks;
    }

    /// <summary>
    /// Confirms pre-verification statuses for this game and optionally all its child entities,
    /// applying cascading rejection logic and clearing warning flags for verified entities
    /// </summary>
    /// <param name="includeChildren">Whether to also confirm pre-verification statuses for child entities</param>
    public void ConfirmPreVerification(bool includeChildren = true)
    {
        VerificationStatus = VerificationStatus.ConfirmPreStatus();

        if (VerificationStatus == VerificationStatus.Verified)
        {
            WarningFlags = GameWarningFlags.None;
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
            Scores.ForEach(score => score.ConfirmPreVerification());
        }
    }

    /// <summary>
    /// Rejects all child entities in this game
    /// </summary>
    public void RejectAllChildren()
    {
        foreach (GameScore score in Scores)
        {
            score.VerificationStatus = VerificationStatus.Rejected;
            score.RejectionReason |= ScoreRejectionReason.RejectedGame;
            score.ProcessingStatus = ScoreProcessingStatus.Done;
        }
    }

    public Ruleset PlayMode { get; set; }
}
