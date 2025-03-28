using Common.Enums;

namespace API.DTOs;

/// <summary>
/// Represents a beatmap's attributes
/// </summary>
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
