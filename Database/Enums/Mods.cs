using System.Diagnostics.CodeAnalysis;

namespace Database.Enums;

/// <summary>
/// Represents score multiplier values for certain <see cref="Mods"/>
/// </summary>
public struct ModScoreMultipliers
{
    /// <summary>
    /// Score multiplier for <see cref="Mods.None"/>
    /// </summary>
    /// <remarks>Also applied for <see cref="Mods.NoFail"/></remarks>
    public const double NoMod = 1;

    /// <summary>
    /// Score multiplier for <see cref="Mods.Easy"/>
    /// </summary>
    public const double Easy = 0.5;

    /// <summary>
    /// Score multiplier for <see cref="Mods.Hidden"/>
    /// </summary>
    public const double Hidden = 1.06;

    /// <summary>
    /// Score multiplier for <see cref="Mods.HardRock"/>
    /// </summary>
    public const double HardRock = 1.1;

    /// <summary>
    /// Score multiplier for <see cref="Mods.HalfTime"/>
    /// </summary>
    public const double HalfTime = 0.3;

    /// <summary>
    /// Score multiplier for <see cref="Mods.DoubleTime"/>
    /// </summary>
    public const double DoubleTime = 1.12;

    /// <summary>
    /// Score multiplier for <see cref="Mods.Flashlight"/>
    /// </summary>
    public const double Flashlight = 1.12;

    /// <summary>
    /// Score multiplier for the combination of <see cref="Mods.Hidden"/> and <see cref="Mods.DoubleTime"/>
    /// </summary>
    public const double HiddenDoubleTime = 1.1872;

    /// <summary>
    /// Score multiplier for the combination of <see cref="Mods.Hidden"/> and <see cref="Mods.HardRock"/>
    /// </summary>
    public const double HiddenHardRock = 1.166;

    /// <summary>
    /// Score multiplier for the combination of <see cref="Mods.Hidden"/> and <see cref="Mods.Easy"/>
    /// </summary>
    public const double HiddenEasy = 0.53;
}

/// <summary>
/// Represents mod values
/// </summary>
/// <copyright>
/// ppy 2024 https://github.com/ppy/osu-api/wiki#mods
/// Last accessed April 2024
/// </copyright>
[Flags]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum Mods
{
    /// <summary>
    /// No mods enabled
    /// </summary>
    None = 0,

    /// <summary>
    /// No fail (NF)
    /// </summary>
    NoFail = 1,

    /// <summary>
    /// Easy (EZ)
    /// </summary>
    Easy = 2,

    /// <summary>
    /// Touch Device (TD)
    /// </summary>
    TouchDevice = 4,

    /// <summary>
    /// Hidden (HD)
    /// </summary>
    Hidden = 8,

    /// <summary>
    /// Hard Rock (HR)
    /// </summary>
    HardRock = 16,

    /// <summary>
    /// Sudden Death (SD)
    /// </summary>
    SuddenDeath = 32,

    /// <summary>
    /// Double Time (DT)
    /// </summary>
    DoubleTime = 64,

    /// <summary>
    /// Relax (RX)
    /// </summary>
    Relax = 128,

    /// <summary>
    /// Half Time (HT)
    /// </summary>
    HalfTime = 256,

    /// <summary>
    /// Nightcore (NC)
    /// </summary>
    /// <remarks>Only set along with DoubleTime. i.e: NC only gives 576</remarks>
    Nightcore = 512,

    /// <summary>
    /// Flashlight (FL)
    /// </summary>
    Flashlight = 1024,

    /// <summary>
    /// Autoplay (AT)
    /// </summary>
    Autoplay = 2048,

    /// <summary>
    /// Spun Out (SO)
    /// </summary>
    SpunOut = 4096,

    /// <summary>
    /// Autopilot (AP)
    /// </summary>
    /// <remarks>Autopilot</remarks>
    Relax2 = 8192,

    /// <summary>
    /// Perfect (PF)
    /// </summary>
    /// <remarks>Only set along with <see cref="SuddenDeath"/>. i.e: PF only gives 16416</remarks>
    Perfect = 16384,

    /// <summary>
    /// 4 key (4K)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    Key4 = 32768,

    /// <summary>
    /// 5 key (5K)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    Key5 = 65536,

    /// <summary>
    /// 6 key (6K)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    Key6 = 131072,

    /// <summary>
    /// 7 key (7K)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    Key7 = 262144,

    /// <summary>
    /// 8 key (8K)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    Key8 = 524288,

    /// <summary>
    /// Fade In (FI)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    FadeIn = 1048576,

    /// <summary>
    /// Random (RD)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    Random = 2097152,

    /// <summary>
    /// Cinema (CM)
    /// </summary>
    Cinema = 4194304,

    /// <summary>
    /// Target Practice (TP)
    /// </summary>
    Target = 8388608,

    /// <summary>
    /// 9 Key (9K)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    Key9 = 16777216,

    /// <summary>
    /// Co-op (CO)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    KeyCoop = 33554432,

    /// <summary>
    /// 1 Key (1K)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    Key1 = 67108864,

    /// <summary>
    /// 3 Key (3K)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    Key3 = 134217728,

    /// <summary>
    /// 2 Key (2K)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    Key2 = 268435456,

    /// <summary>
    /// Score v2 (SV2)
    /// </summary>
    ScoreV2 = 536870912,

    /// <summary>
    /// Mirror (MR)
    /// </summary>
    /// <remarks>Applicable only to <see cref="Ruleset.ManiaOther"/></remarks>
    Mirror = 1073741824,

    /// <summary>
    /// Denotes mods that are <see cref="Ruleset.ManiaOther"/> key modifiers
    /// </summary>
    /// <remarks>See https://osu.ppy.sh/wiki/en/Gameplay/Game_modifier/xK</remarks>
    KeyMod = Key1 | Key2 | Key3 | Key4 | Key5 | Key6 | Key7 | Key8 | Key9 | KeyCoop,

    /// <summary>
    /// Denotes mods that are available to use during Free Mod settings
    /// </summary>
    FreeModAllowed =
        NoFail
        | Easy
        | Hidden
        | HardRock
        | SuddenDeath
        | Flashlight
        | FadeIn
        | Relax
        | Relax2
        | SpunOut
        | KeyMod,

    /// <summary>
    /// Denotes mods that directly impose a modifier on score
    /// </summary>
    ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn
}
