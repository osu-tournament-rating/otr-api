using System.Diagnostics.CodeAnalysis;

namespace API.Osu.Enums;

/// <summary>
/// Represents the four osu! play modes
/// </summary>
[SuppressMessage("ReSharper", "IdentifierTypo")]
public enum Ruleset
{
    /// <summary>
    /// osu! Standard
    /// </summary>
    Standard = 0,

    /// <summary>
    /// osu! Taiko
    /// </summary>
    Taiko = 1,

    /// <summary>
    /// osu! Catch (aka Fruits)
    /// </summary>
    Catch = 2,

    /// <summary>
    /// osu! Mania
    /// </summary>
    Mania = 3
}
