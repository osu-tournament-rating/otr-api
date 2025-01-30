using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Core beatmap information
/// </summary>
[Table("beatmaps")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class Beatmap : UpdateableEntityBase
{
    /// <summary>
    /// Id of the associated beatmapset
    /// </summary>
    [Column("beatmapset_id")]
    public int? BeatmapSetId { get; set; }

    /// <summary>
    /// osu! beatmap ID
    /// </summary>
    [Column("osu_id")]
    public long OsuId { get; set; }

    /// <summary>
    /// Denotes if the <see cref="Beatmap"/> has populated data
    /// </summary>
    /// <remarks>
    /// Set only on creation. If the beatmap is deleted from osu! at
    /// the time of access, this value will be false and all properties will be unpopulated.
    /// </remarks>
    [Column("has_data")]
    public bool HasData { get; set; }

    /// <summary>
    /// Ruleset
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Ranked status
    /// </summary>
    [Column("ranked_status")]
    public BeatmapRankedStatus RankedStatus { get; set; }

    /// <summary>
    /// Difficulty name
    /// </summary>
    [Column("diff_name")]
    [MaxLength(512)]
    public string DiffName { get; set; } = string.Empty;

    /// <summary>
    /// Total length in seconds
    /// </summary>
    [Column("total_length")]
    public long TotalLength { get; set; }

    /// <summary>
    /// Drain length in seconds
    /// </summary>
    [Column("drain_length")]
    public int DrainLength { get; set; }

    /// <summary>
    /// Beats per minute
    /// </summary>
    [Column("bpm")]
    public double Bpm { get; set; }

    /// <summary>
    /// Number of circles in the beatmap
    /// </summary>
    [Column("count_circle")]
    public int CountCircle { get; set; }

    /// <summary>
    /// Number of sliders in the beatmap
    /// </summary>
    [Column("count_slider")]
    public int CountSlider { get; set; }

    /// <summary>
    /// Number of spinners in the beatmap
    /// </summary>
    [Column("count_spinner")]
    public int CountSpinner { get; set; }

    /// <summary>
    /// Circle size
    /// </summary>
    [Column("cs")]
    public double Cs { get; set; }

    /// <summary>
    /// HP drain rate
    /// </summary>
    [Column("hp")]
    public double Hp { get; set; }

    /// <summary>
    /// Overall difficulty
    /// </summary>
    [Column("od")]
    public double Od { get; set; }

    /// <summary>
    /// Approach rate
    /// </summary>
    [Column("ar")]
    public double Ar { get; set; }

    /// <summary>
    /// Star rating (No Mod)
    /// </summary>
    [Column("sr")]
    public double Sr { get; set; }

    /// <summary>
    /// Maximum combo, if available
    /// </summary>
    [Column("max_combo")]
    public int? MaxCombo { get; set; }

    /// <summary>
    /// The associated beatmapset, if available
    /// </summary>
    public BeatmapSet? BeatmapSet { get; set; }

    /// <summary>
    /// Collection of players who created this beatmap
    /// </summary>
    public ICollection<Player> Creators { get; set; } = [];

    /// <summary>
    /// Collection of games played on this beatmap
    /// </summary>
    public ICollection<Game> Games { get; set; } = [];

    /// <summary>
    /// Collection of tournaments this beatmap is pooled in
    /// </summary>
    public ICollection<Tournament> TournamentsPooledIn { get; set; } = [];

    /// <summary>
    /// Collection of attributes for this beatmap
    /// </summary>
    public ICollection<BeatmapAttributes> Attributes { get; set; } = [];
}
