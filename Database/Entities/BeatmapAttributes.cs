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
    public Mods Mods { get; init; }

    /// <summary>
    /// Star rating of the beatmap with the applied mods
    /// </summary>
    public double Sr { get; init; }

    /// <summary>
    /// Id of the associated beatmap
    /// </summary>
    public int BeatmapId { get; init; }

    /// <summary>
    /// The associated beatmap
    /// </summary>
    public Beatmap Beatmap { get; init; } = null!;
}
