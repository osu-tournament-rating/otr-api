﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Common.Enums.Verification;
using Common.Utilities;
using Database.Entities.Interfaces;
using Database.Utilities;

namespace Database.Entities;

/// <summary>
/// A score set by a <see cref="Entities.Player"/> in a <see cref="Entities.Game"/>
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class GameScore : UpdateableEntityBase, IProcessableEntity, IAdminNotableEntity<GameScoreAdminNote>,
    IAuditableEntity<GameScoreAudit>, IScoreStatistics
{
    /// <summary>
    /// Total score
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Placement of the <see cref="Score"/> in the <see cref="Game"/> compared to all <see cref="Entities.Player"/>'s <see cref="Score"/>
    /// </summary>
    public int Placement { get; set; }

    /// <summary>
    /// Max combo obtained
    /// </summary>
    public int MaxCombo { get; set; }

    public int Count50 { get; set; }

    public int Count100 { get; set; }

    public int Count300 { get; set; }

    public int CountMiss { get; set; }

    public int CountKatu { get; set; }

    public int CountGeki { get; set; }

    /// <summary>
    /// Denotes if the <see cref="Player"/> passed
    /// </summary>
    public bool Pass { get; set; }

    /// <summary>
    /// Denotes if the score is perfect (is an SS)
    /// </summary>
    public bool Perfect { get; set; }

    /// <summary>
    /// Represents the overall performance as a letter grade
    /// </summary>
    public ScoreGrade Grade { get; set; }

    /// <summary>
    /// The <see cref="Common.Enums.Mods"/> enabled for the score
    /// </summary>
    public Mods Mods { get; set; }

    /// <summary>
    /// The <see cref="Common.Enums.Team"/> the <see cref="Player"/> played for in the game
    /// </summary>
    public Team Team { get; set; }

    /// <summary>
    /// The <see cref="Common.Enums.Ruleset"/> the score was set in
    /// </summary>
    public Ruleset Ruleset { get; set; }

    public VerificationStatus VerificationStatus { get; set; }

    [AuditIgnore]
    public DateTime LastProcessingDate { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    public ScoreRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    public ScoreProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// Id of the <see cref="Entities.Game" /> that the <see cref="GameScore" /> was set in
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Game"/> that the <see cref="GameScore"/> was set in
    /// </summary>
    public Game Game { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="Entities.Player" /> that set the <see cref="GameScore" />
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Player"/> that set the <see cref="GameScore"/>
    /// </summary>
    public Player Player { get; set; } = null!;

    public ICollection<GameScoreAdminNote> AdminNotes { get; set; } = [];

    public ICollection<GameScoreAudit> Audits { get; set; } = [];

    [NotMapped] public int? ActionBlamedOnUserId { get; set; }

    /// <summary>
    /// Accuracy
    /// </summary>
    /// <remarks>
    /// Represented as a full percentage, e.g. 98.5 (instead of 0.985) <br/>
    /// See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Accuracy">osu! wiki - Accuracy</a>
    /// </remarks>
    [NotMapped]
    public double Accuracy
    {
        get
        {
            double divisor;
            switch (Ruleset)
            {
                case Ruleset.Osu:
                    divisor = 300d * (Count300 + Count100 + Count50 + CountMiss);
                    return divisor != 0
                        ? 100 * (300d * Count300 + 100d * Count100 + 50d * Count50) / divisor
                        : divisor;

                case Ruleset.Taiko:
                    divisor = Count300 + Count100 + CountMiss;
                    return divisor != 0
                        ? 100 * (Count300 + 0.5 * Count100) / divisor
                        : divisor;

                case Ruleset.Catch:
                    divisor = Count300 + Count100 + Count50 + CountMiss + CountKatu;
                    return divisor != 0
                        ? 100 * (Count300 + Count100 + Count50) / divisor
                        : divisor;

                // Any mania variant
                default:
                    divisor = 305d * (CountGeki + Count300 + CountKatu + Count100 + Count50 + CountMiss);
                    return divisor != 0
                        ? 100 * (305d * CountGeki + 300 * Count300 + 200 * CountKatu + 100 * Count100 + 50 * Count50) /
                          divisor
                        : divisor;
            }
        }
    }

    public void ResetAutomationStatuses(bool force)
    {
        var scoreUpdate = force || (VerificationStatus != VerificationStatus.Rejected &&
                                    VerificationStatus != VerificationStatus.Verified);

        if (!scoreUpdate)
        {
            return;
        }

        VerificationStatus = VerificationStatus.None;
        RejectionReason = ScoreRejectionReason.None;
        ProcessingStatus = ScoreProcessingStatus.NeedsAutomationChecks;
    }

    public void ConfirmPreVerificationStatus() => VerificationStatus = EnumUtils.ConfirmPreStatus(VerificationStatus);
}
