using Common.Enums;
using JetBrains.Annotations;

namespace Database.Entities;

/// <summary>
/// Represents a beatmap's attributes from the osu! API
/// </summary>
[UsedImplicitly]
public class BeatmapAttributes : EntityBase
{
    /// <summary>
    /// Mods applied to the beatmap
    /// </summary>
    public Mods Mods { get; set; }

    /// <summary>
    /// Star rating of the beatmap with the applied mods
    /// </summary>
    public double Sr { get; set; }

    /// <summary>
    /// Id of the associated beatmap
    /// </summary>
    public int BeatmapId { get; set; }

    /// <summary>
    /// The associated beatmap
    /// </summary>
    public Beatmap Beatmap { get; set; } = null!;
}
