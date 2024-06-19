using System.Diagnostics.CodeAnalysis;

namespace Database.Enums;

/// <summary>
/// Represents the judgement statistics of a score as a letter grade
/// </summary>
/// <remarks>
/// See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Grade">osu! Grade</a>
/// Summaries are provided as per the <see cref="Ruleset.Standard"/> requirements, but are calculated by <see cref="Ruleset"/>
/// </remarks>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum ScoreGrade
{
    /// <summary>
    /// 100% accuracy with <see cref="Mods.Hidden"/> and/or <see cref="Mods.Flashlight"/>
    /// </summary>
    SSH,

    /// <summary>
    /// Over 90% 300s, less than 1% 50s and no misses with <see cref="Mods.Hidden"/> and/or <see cref="Mods.Flashlight"/>
    /// </summary>
    SH,

    /// <summary>
    /// 100% accuracy
    /// </summary>
    SS,

    /// <summary>
    /// Over 90% 300s, less than 1% 50s and no misses
    /// </summary>
    S,

    /// <summary>
    /// Over 80% 300s and no misses OR over 90% 300s
    /// </summary>
    A,

    /// <summary>
    /// Over 70% 300s and no misses OR over 80% 300s
    /// </summary>
    B,

    /// <summary>
    /// Over 60% 300s
    /// </summary>
    C,

    /// <summary>
    /// Anything else
    /// </summary>
    D
}
