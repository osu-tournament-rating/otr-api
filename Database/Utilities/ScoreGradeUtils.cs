using System.Diagnostics.CodeAnalysis;
using Common.Enums.Enums;
using Database.Entities;
using Database.Entities.Interfaces;

namespace Database.Utilities;

/// <summary>
/// Helper for determining the <see cref="ScoreGrade"/> of a <see cref="GameScore"/>
/// </summary>
/// <remarks>
/// Calculations based on the <a href="https://osu.ppy.sh/wiki/en/Gameplay/Grade">osu! wiki - Grade</a>
/// </remarks>
public static class ScoreGradeUtils
{
    /// <summary>
    /// Determines the <see cref="ScoreGrade"/> of a <see cref="GameScore"/>
    /// played in <see cref="Ruleset.Osu"/>
    /// </summary>
    /// <param name="accuracy">Accuracy</param>
    /// <param name="mods">Enabled mods</param>
    /// <param name="statistics">Judgement statistics</param>
    /// <returns>The determined <see cref="ScoreGrade"/></returns>
    public static ScoreGrade DetermineStandardGrade(
        double accuracy,
        Mods mods,
        IScoreStatistics statistics
    )
    {
        var totalHits = statistics.Count50 + statistics.Count100 + statistics.Count300 + statistics.CountMiss;
        var percentage300 = (double)statistics.Count300 / totalHits * 100;
        var percentage50 = (double)statistics.Count50 / totalHits * 100;

        return (accuracy, percentage300, percentage50, statistics.CountMiss) switch
        {
            // 100% accuracy (with HD or FL)
            (1.0, _, _, _)
                => mods.HasFlag(Mods.Hidden) || mods.HasFlag(Mods.Flashlight) ? ScoreGrade.SSH : ScoreGrade.SS,
            // Over 90% 300s, less than 1% 50s and no misses (with HD or FL)
            (_, > 90, < 1, 0)
                => mods.HasFlag(Mods.Hidden) || mods.HasFlag(Mods.Flashlight) ? ScoreGrade.SH : ScoreGrade.S,
            // Over 80% 300s and no misses
            (_, > 80, _, 0) => ScoreGrade.A,
            // Over 90% 300s
            (_, > 90, _, _) => ScoreGrade.A,
            // Over 70% 300s and no misses
            (_, > 70, _, 0) => ScoreGrade.B,
            // Over 80% 300s
            (_, > 80, _, _) => ScoreGrade.B,
            // Over 60% 300s
            (_, > 60, _, _) => ScoreGrade.C,
            // Anything else
            _ => ScoreGrade.D
        };
    }

    /// <summary>
    /// Determines the <see cref="ScoreGrade"/> of a <see cref="GameScore"/>
    /// played in <see cref="Ruleset.Taiko"/>
    /// </summary>
    /// <param name="accuracy">Accuracy</param>
    /// <param name="mods">Enabled mods</param>
    /// <param name="statistics">Judgement statistics</param>
    /// <returns>The determined <see cref="ScoreGrade"/></returns>
    [SuppressMessage("ReSharper", "CommentTypo")]
    public static ScoreGrade DetermineTaikoGrade(
        double accuracy,
        Mods mods,
        IScoreStatistics statistics
    )
    {
        var totalHits = statistics.Count50 + statistics.Count100 + statistics.Count300 + statistics.CountMiss;
        var percentage300 = (double)statistics.Count300 / totalHits * 100;

        return (accuracy, percentage300, statistics.CountMiss) switch
        {
            // 100% accuracy (with HD or FL)
            (1.0, _, _)
                => mods.HasFlag(Mods.Hidden) || mods.HasFlag(Mods.Flashlight) ? ScoreGrade.SSH : ScoreGrade.SS,
            // Over 90% GREATs and no misses (with HD or FL)
            (_, > 90, 0)
                => mods.HasFlag(Mods.Hidden) || mods.HasFlag(Mods.Flashlight) ? ScoreGrade.SSH : ScoreGrade.SS,
            // Over 80% GREATs and no misses
            (_, > 80, 0) => ScoreGrade.A,
            // Over 90% GREATs
            (_, > 90, _) => ScoreGrade.A,
            // Over 70% GREATs and no misses
            (_, > 70, 0) => ScoreGrade.B,
            // Over 80% GREATs
            (_, > 80, _) => ScoreGrade.B,
            // Over 60% GREATs
            (_, > 60, _) => ScoreGrade.C,
            // Anything else
            _ => ScoreGrade.D
        };
    }

    /// <summary>
    /// Determines the <see cref="ScoreGrade"/> of a <see cref="GameScore"/>
    /// played in <see cref="Ruleset.Catch"/>
    /// </summary>
    /// <param name="accuracy">Accuracy</param>
    /// <param name="mods">Enabled mods</param>
    /// <returns>The determined <see cref="ScoreGrade"/></returns>
    public static ScoreGrade DetermineCatchGrade(
        double accuracy,
        Mods mods
    )
    {
        return accuracy switch
        {
            // 100% accuracy (with HD or FL)
            1.0 => mods.HasFlag(Mods.Hidden) || mods.HasFlag(Mods.Flashlight) ? ScoreGrade.SSH : ScoreGrade.SS,
            // 98.01% to 99.99% accuracy (with HD or FL)
            > 0.98 => mods.HasFlag(Mods.Hidden) || mods.HasFlag(Mods.Flashlight) ? ScoreGrade.SH : ScoreGrade.S,
            // 94.01% to 98.00% accuracy
            > 0.94 => ScoreGrade.A,
            // 90.01% to 94.00% accuracy
            > 0.9 => ScoreGrade.B,
            // 85.01% to 90.00% accuracy
            > 0.85 => ScoreGrade.C,
            // Any other accuracy under 85.00%
            _ => ScoreGrade.D
        };
    }

    /// <summary>
    /// Determines the <see cref="ScoreGrade"/> of a <see cref="GameScore"/>
    /// played in <see cref="Ruleset.ManiaOther"/>
    /// </summary>
    /// <param name="accuracy">Accuracy</param>
    /// <param name="mods">Enabled mods</param>
    /// <returns>The determined <see cref="ScoreGrade"/></returns>
    public static ScoreGrade DetermineManiaGrade(
        double accuracy,
        Mods mods
    )
    {
        return accuracy switch
        {
            // 100% accuracy (with HD or FL)
            1.0 => mods.HasFlag(Mods.Hidden) || mods.HasFlag(Mods.Flashlight) ? ScoreGrade.SSH : ScoreGrade.SS,
            // Over 95% accuracy (with HD or FL)
            > 0.95 => mods.HasFlag(Mods.Hidden) || mods.HasFlag(Mods.Flashlight) ? ScoreGrade.SH : ScoreGrade.S,
            // Over 90% accuracy
            > 0.9 => ScoreGrade.A,
            // Over 80% accuracy
            > 0.8 => ScoreGrade.B,
            // Over 70% accuracy
            > 0.7 => ScoreGrade.C,
            // Anything else
            _ => ScoreGrade.D
        };
    }
}
