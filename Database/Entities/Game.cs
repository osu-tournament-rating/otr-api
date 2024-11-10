using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Interfaces;
using Database.Enums;
using Database.Enums.Verification;

namespace Database.Entities;

/// <summary>
/// A game played in a <see cref="Match"/>
/// </summary>
[Table("games")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Game : UpdateableEntityBase, IProcessableEntity, IAdminNotableEntity<GameAdminNote>, IAuditableEntity<GameAudit>
{
    /// <summary>
    /// osu! id
    /// </summary>
    [Column("osu_id")]
    public long OsuId { get; set; }

    /// <summary>
    /// The <see cref="Enums.Ruleset"/> the game was played in
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// The <see cref="Enums.ScoringType"/> used
    /// </summary>
    [Column("scoring_type")]
    public ScoringType ScoringType { get; set; }

    /// <summary>
    /// The <see cref="Enums.TeamType"/> used
    /// </summary>
    [Column("team_type")]
    public TeamType TeamType { get; set; }

    /// <summary>
    /// The <see cref="Enums.Mods"/> enabled for the game
    /// </summary>
    [Column("mods")]
    public Mods Mods { get; set; }

    /// <summary>
    /// Timestamp for the beginning of the game
    /// </summary>
    [Column("start_time")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Timestamp for the end of the game
    /// </summary>
    [Column("end_time")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Verification status
    /// </summary>
    [Column("verification_status")]
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    [Column("rejection_reason")]
    public GameRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Warning flags
    /// </summary>
    [Column("warning_flags")]
    public GameWarningFlags WarningFlags { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    [Column("processing_status")]
    public GameProcessingStatus ProcessingStatus { get; set; }

    [Column("last_processing_date")]
    public DateTime LastProcessingDate { get; set; }

    /// <summary>
    /// Id of the <see cref="Entities.Match"/> that the game was played in
    /// </summary>
    [Column("match_id")]
    public int MatchId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Match"/> the game was played in
    /// </summary>
    public Match Match { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="Entities.Beatmap"/> played during the game
    /// </summary>
    [Column("beatmap_id")]
    public int? BeatmapId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Beatmap"/> played during the game
    /// </summary>
    public Beatmap? Beatmap { get; set; }

    /// <summary>
    /// The win record for the game
    /// </summary>
    public GameWinRecord? WinRecord { get; set; }

    /// <summary>
    /// A collection of <see cref="GameScore"/>s set in the <see cref="Game"/>
    /// </summary>
    public ICollection<GameScore> Scores { get; set; } = new List<GameScore>();

    public ICollection<GameAdminNote> AdminNotes { get; set; } = new List<GameAdminNote>();

    public ICollection<GameAudit> Audits { get; set; } = new List<GameAudit>();


    [NotMapped]
    public int? ActionBlamedOnUserId { get; set; }

    /// <summary>
    /// Denotes if the mod setting was "free mod"
    /// </summary>
    [NotMapped]
    public bool IsFreeMod => Mods is Mods.None;

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
}
