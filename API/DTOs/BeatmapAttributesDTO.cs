using Common.Enums;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a beatmap's attributes
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class BeatmapAttributesDTO
{
    /// <summary>
    /// Mods applied
    /// </summary>
    public Mods Mods { get; set; }

    /// <summary>
    /// Star rating with applied mods
    /// </summary>
    public double Sr { get; set; }
}
