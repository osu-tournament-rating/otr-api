using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Enums;
// ReSharper disable CollectionNeverUpdated.Global

namespace Database.Entities;

/// <summary>
/// Represents a beatmap in the osu! API.
/// </summary>
[Table("beatmaps")]
public class Beatmap : UpdateableEntityBase
{
    /// <summary>
    /// osu! beatmap ID
    /// </summary>
    [Column("osu_id")]
    public long OsuId { get; init; }

    /// <summary>
    /// Denotes if the <see cref="Beatmap"/> has populated data
    /// </summary>
    /// <remarks>
    /// Set only on creation. If the beatmap is deleted from osu! at
    /// the time of access, this value will be false and all properties will be unpopulated.
    /// </remarks>
    [Column("has_data")]
    public bool HasData { get; init; }

    /// <summary>
    /// Ruleset
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Difficulty name
    /// </summary>
    [Column("diff_name")]
    [MaxLength(512)]
    public string DiffName { get; init; } = string.Empty;

    /// <summary>
    /// Total length in seconds
    /// </summary>
    [Column("total_length")]
    public int TotalLength { get; init; }

    /// <summary>
    /// Drain length in seconds
    /// </summary>
    [Column("drain_length")]
    public int DrainLength { get; init; }

    /// <summary>
    /// Beats per minute
    /// </summary>
    [Column("bpm")]
    public double Bpm { get; init; }

    /// <summary>
    /// Number of circles in the beatmap
    /// </summary>
    [Column("count_circle")]
    public int CountCircle { get; init; }

    /// <summary>
    /// Number of sliders in the beatmap
    /// </summary>
    [Column("count_slider")]
    public int CountSlider { get; init; }

    /// <summary>
    /// Number of spinners in the beatmap
    /// </summary>
    [Column("count_spinner")]
    public int CountSpinner { get; init; }

    /// <summary>
    /// Circle size
    /// </summary>
    [Column("cs")]
    public double Cs { get; init; }

    /// <summary>
    /// HP drain rate
    /// </summary>
    [Column("hp")]
    public double Hp { get; init; }

    /// <summary>
    /// Overall difficulty
    /// </summary>
    [Column("od")]
    public double Od { get; init; }

    /// <summary>
    /// Approach rate
    /// </summary>
    [Column("ar")]
    public double Ar { get; init; }

    /// <summary>
    /// Star rating (No Mod)
    /// </summary>
    [Column("sr")]
    public double Sr { get; init; }

    /// <summary>
    /// Maximum combo, if available
    /// </summary>
    [Column("max_combo")]
    public int? MaxCombo { get; init; }

    /// <summary>
    /// Id of the associated beatmapset
    /// </summary>
    [Column("beatmapset_id")]
    public int BeatmapSetId { get; init; }

    /// <summary>
    /// The associated beatmapset
    /// </summary>
    public BeatmapSet BeatmapSet { get; init; } = null!;

    /// <summary>
    /// Collection of players who created this beatmap
    /// </summary>
    public ICollection<Player> Creators { get; init; } = [];

    /// <summary>
    /// Collection of games played on this beatmap
    /// </summary>
    public ICollection<Game> Games { get; init; } = [];

    /// <summary>
    /// Collection of tournaments this beatmap is pooled in
    /// </summary>
    public ICollection<Tournament> TournamentsPooledIn { get; init; } = [];

    /// <summary>
    /// Collection of attributes for this beatmap
    /// </summary>
    public ICollection<BeatmapAttributes> Attributes { get; init; } = [];
}
