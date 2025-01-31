using System.ComponentModel.DataAnnotations.Schema;
using Database.Enums;
using JetBrains.Annotations;

namespace Database.Entities;

/// <summary>
/// Represents a beatmap's attributes from the osu! API
/// </summary>
[UsedImplicitly]
[Table("beatmap_attributes")]
public class BeatmapAttributes : EntityBase
{
    /// <summary>
    /// Mods applied to the beatmap
    /// </summary>
    [Column("mods")]
    public Mods Mods { get; set; }

    /// <summary>
    /// Star rating of the beatmap with the applied mods
    /// </summary>
    [Column("sr")]
    public double Sr { get; set; }

    /// <summary>
    /// Id of the associated beatmap
    /// </summary>
    [Column("beatmap_id")]
    public int BeatmapId { get; set; }

    /// <summary>
    /// The associated beatmap
    /// </summary>
    public Beatmap Beatmap { get; set; } = null!;
}
