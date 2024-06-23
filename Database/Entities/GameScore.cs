using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Interfaces;
using Database.Enums;
using Database.Enums.Verification;

namespace Database.Entities;

/// <summary>
/// A score set by a <see cref="Entities.Player"/> in a <see cref="Entities.Game"/>
/// </summary>
[Table("game_scores")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class GameScore : UpdateableEntityBase, IScoreStatistics
{
    /// <summary>
    /// Total score
    /// </summary>
    [Column("score")]
    public long Score { get; set; }

    /// <summary>
    /// Max combo obtained
    /// </summary>
    [Column("max_combo")]
    public int MaxCombo { get; set; }

    [Column("count_50")]
    public int Count50 { get; set; }

    [Column("count_100")]
    public int Count100 { get; set; }

    [Column("count_300")]
    public int Count300 { get; set; }

    [Column("count_miss")]
    public int CountMiss { get; set; }

    [Column("count_katu")]
    public int CountKatu { get; set; }

    [Column("count_geki")]
    public int CountGeki { get; set; }

    /// <summary>
    /// Denotes if the <see cref="Player"/> passed
    /// </summary>
    [Column("pass")]
    public bool Pass { get; set; }

    /// <summary>
    /// Denotes if the score is perfect (is an SS)
    /// </summary>
    [Column("perfect")]
    public bool Perfect { get; set; }

    /// <summary>
    /// Represents the overall performance as a letter grade
    /// </summary>
    [Column("grade")]
    public ScoreGrade Grade { get; set; }

    /// <summary>
    /// The <see cref="Enums.Mods"/> enabled for the score
    /// </summary>
    [Column("mods")]
    public Mods Mods { get; set; }

    /// <summary>
    /// The <see cref="Enums.Team"/> the <see cref="Player"/> played for in the game
    /// </summary>
    [Column("team")]
    public Team Team { get; set; }

    /// <summary>
    /// The <see cref="Enums.Ruleset"/> the score was set in
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Verification status
    /// </summary>
    [Column("verification_status")]
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    [Column("rejection_reason")]
    public ScoreRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Id of the <see cref="Entities.Game"/> that the <see cref="GameScore"/> was set in
    /// </summary>
    [Column("game_id")]
    public int GameId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Game"/> that the <see cref="GameScore"/> was set in
    /// </summary>
    public Game Game { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="Entities.Player"/> that set the <see cref="GameScore"/>
    /// </summary>
    [Column("player_id")]
    public int PlayerId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Player"/> that set the <see cref="GameScore"/>
    /// </summary>
    public Player Player { get; set; } = null!;

    /// <summary>
    /// Accuracy represented as a full percentage, e.g. 98.5 (instead of 0.985)
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Accuracy">osu! Accuracy</a></remarks>
    [NotMapped]
    public double AccuracyStandard
    {
        get
        {
            var divisor = 300d * (Count300 + Count100 + Count50 + CountMiss);

            if (divisor == 0)
            {
                return 0;
            }

            return (100 * ((300d * Count300) + (100d * Count100) + (50d * Count50))) / divisor;
        }
    }

    /// <summary>
    /// Accuracy represented as a full percentage, e.g. 98.5 (instead of 0.985)
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Accuracy">osu! Accuracy</a></remarks>
    [NotMapped]
    public double AccuracyTaiko
    {
        get
        {
            double divisor = Count300 + Count100 + CountMiss;

            if (divisor == 0)
            {
                return 0;
            }

            return (100 * (Count300 + (0.5 * Count100))) / divisor;
        }
    }

    /// <summary>
    /// Accuracy represented as a full percentage, e.g. 98.5 (instead of 0.985).
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Accuracy">osu! Accuracy</a></remarks>
    [NotMapped]
    public double AccuracyCatch
    {
        get
        {
            var nFruitsCaught = Count300;
            var nDropsCaught = Count100;
            var nDropletsCaught = Count50;

            double divisor = nFruitsCaught + nDropsCaught + nDropletsCaught + CountMiss + CountKatu;

            if (divisor == 0)
            {
                return 0;
            }

            return (100 * (Count300 + Count100 + Count50)) / divisor;
        }
    }

    /// <summary>
    /// Accuracy represented as a full percentage, e.g. 98.5 (instead of 0.985). ScoreV2 accuracy as shown here
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Accuracy">osu! Accuracy</a></remarks>
    [NotMapped]
    public double AccuracyMania
    {
        get
        {
            var divisor = 305d * (CountGeki + Count300 + CountKatu + Count100 + Count50 + CountMiss);

            if (divisor == 0)
            {
                return 0;
            }

            return (
                    100
                    * (
                        (305d * CountGeki)
                        + (300 * Count300)
                        + (200 * CountKatu)
                        + (100 * Count100)
                        + (50 * Count50)
                    )
                ) / divisor;
        }
    }
}
