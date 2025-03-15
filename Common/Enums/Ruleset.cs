using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Common.Enums;

/// <summary>
/// Represents osu! play modes
/// </summary>
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum Ruleset
{
    /// <summary>
    /// osu! (standard)
    /// </summary>
    [Description("osu")]
    Osu = 0,

    /// <summary>
    /// osu! Taiko
    /// </summary>
    [Description("taiko")]
    Taiko = 1,

    /// <summary>
    /// osu! Catch (aka Fruits)
    /// </summary>
    [Description("fruits")]
    Catch = 2,

    /// <summary>
    /// osu! Mania
    /// </summary>
    /// <remarks>
    /// Encompasses all of the osu!mania ruleset and represents a ruleset that has
    /// not yet been identified as either <see cref="Mania4k"/> or <see cref="Mania7k"/>
    /// </remarks>
    [Description("mania")]
    ManiaOther = 3,

    /// <summary>
    /// osu! Mania 4k variant
    /// </summary>
    Mania4k = 4,

    /// <summary>
    /// osu! Mania 7k variant
    /// </summary>
    Mania7k = 5
}
