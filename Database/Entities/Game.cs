using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Interfaces;
using Database.Enums;
using Database.Enums.Verification;
using Database.Utilities;

namespace Database.Entities;

/// <summary>
/// A game played in a <see cref="Match"/>
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Game : UpdateableEntityBase, IProcessableEntity, IAdminNotableEntity<GameAdminNote>,
    IAuditableEntity<GameAudit>
{
    /// <summary>
    /// osu! id
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// The <see cref="Enums.Ruleset"/> the game was played in
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// The <see cref="Enums.ScoringType"/> used
    /// </summary>
    public ScoringType ScoringType { get; set; }

    /// <summary>
    /// The <see cref="Enums.TeamType"/> used
    /// </summary>
    public TeamType TeamType { get; set; }

    /// <summary>
    /// The <see cref="Enums.Mods"/> enabled for the game
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

    public ICollection<GameAdminNote> AdminNotes { get; set; } = [];

    public ICollection<GameAudit> Audits { get; set; } = [];

    [NotMapped] public int? ActionBlamedOnUserId { get; set; }

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

    public void ResetAutomationStatuses(bool force)
    {
        var gameUpdate = force || (VerificationStatus != VerificationStatus.Rejected &&
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

    public void ConfirmPreVerificationStatus() => VerificationStatus = EnumUtils.ConfirmPreStatus(VerificationStatus);
}
