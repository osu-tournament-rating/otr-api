using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Interfaces;
using Database.Enums;

namespace Database.Entities;

// TODO: Rename to "Score"
/// <summary>
/// Describes performance information relative to a single <see cref="Entities.Game"/> for a <see cref="Entities.Player"/>
/// </summary>
/// <remarks>
/// Functionally, a <see cref="MatchScore"/> is a representation of a single "map" played in a
/// <see cref="Entities.Match"/> by a <see cref="Entities.Player"/>
/// </remarks>
[Table("match_scores")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class MatchScore : UpdateableEntityBase, IScoreStatistics
{
    /// <summary>
    /// The <see cref="Enums.Team"/> the <see cref="Player"/> played for in the match
    /// </summary>
    [Column("team")]
    public int Team { get; set; }

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

    /// <summary>
    /// Count of combos completed without the highest possible accuracy on every note
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Judgement/Katu">osu! Judgement - Katu</a></remarks>
    [Column("count_katu")]
    public int CountKatu { get; set; }

    /// <summary>
    /// Count of combos completed with the highest possible accuracy on every note
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Judgement/Geki">osu! Judgement - Geki</a></remarks>
    [Column("count_geki")]
    public int CountGeki { get; set; }

    /// <summary>
    /// Denotes if the score is perfect (AKA, is an SS)
    /// </summary>
    [Column("pass")]
    public bool Pass { get; set; }

    /// <summary>
    /// Denotes if the <see cref="Player"/> passed
    /// </summary>
    [Column("perfect")]
    public bool Perfect { get; set; }

    /// <summary>
    /// The <see cref="Mods"/> enabled for the score represented as an integer
    /// </summary>
    /// <remarks>
    /// Mods are only populated at the <see cref="MatchScore"/> level for <see cref="Entities.Game"/>s played with
    /// "FreeMod". If the <see cref="Game"/> was forced "ScoreV2 + NoFail", <see cref="MatchScore"/> mods will be null,
    /// and mods should be referenced from the <see cref="Game"/> instead.
    /// </remarks>
    [Column("enabled_mods")]
    public int? EnabledMods { get; set; }

    // TODO: REMOVE
    /// <summary>
    /// If not valid, the score is not sent to the rating processor.
    /// </summary>
    [Column("is_valid")]
    public bool? IsValid { get; set; }

    /// <summary>
    /// Id of the <see cref="Entities.Game"/> that the <see cref="MatchScore"/> was a part of
    /// </summary>
    [Column("game_id")]
    public int GameId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Game"/> that the <see cref="MatchScore"/> was a part of
    /// </summary>
    public Game Game { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="Entities.Player"/> that owns the <see cref="MatchScore"/>
    /// </summary>
    [Column("player_id")]
    public int PlayerId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Player"/> that owns the <see cref="MatchScore"/>
    /// </summary>
    public Player Player { get; set; } = null!;

    [NotMapped]
    public Mods? EnabledModsEnum
    {
        get
        {
            if (EnabledMods != null)
            {
                return (Mods)EnabledMods;
            }

            return null;
        }
    }

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
