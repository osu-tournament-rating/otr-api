using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// An osu! beatmap
/// </summary>
[Table("beatmaps")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Beatmap : EntityBase
{
    /// <summary>
    /// osu! id of the beatmap
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
    /// osu! id of the mapper
    /// </summary>
    [Column("mapper_id")]
    public long MapperId { get; set; }

    /// <summary>
    /// osu! username of the mapper
    /// </summary>
    [MaxLength(32)]
    [Column("mapper_name")]
    public string MapperName { get; set; } = string.Empty;

    /// <summary>
    /// Song artist
    /// </summary>
    [MaxLength(512)]
    [Column("artist")]
    public string Artist { get; set; } = string.Empty;

    /// <summary>
    /// Song title
    /// </summary>
    [MaxLength(512)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Difficulty name
    /// </summary>
    [MaxLength(255)]
    [Column("diff_name")]
    public string DiffName { get; set; } = string.Empty;

    /// <summary>
    /// Ranked status
    /// </summary>
    [Column("ranked_status")]
    public BeatmapRankedStatus RankedStatus { get; set; }

    /// <summary>
    /// Star rating
    /// </summary>
    [Column("sr")]
    public double Sr { get; set; }

    /// <summary>
    /// Beats per minute
    /// </summary>
    [Column("bpm")]
    public double Bpm { get; set; }

    /// <summary>
    /// Circle size
    /// </summary>
    [Column("cs")]
    public double Cs { get; set; }

    /// <summary>
    /// Approach rate
    /// </summary>
    [Column("ar")]
    public double Ar { get; set; }

    /// <summary>
    /// Hp
    /// </summary>
    [Column("hp")]
    public double Hp { get; set; }

    /// <summary>
    /// Overall difficulty
    /// </summary>
    [Column("od")]
    public double Od { get; set; }

    /// <summary>
    /// Total length of the song
    /// </summary>
    [Column("length")]
    public double Length { get; set; }

    /// <summary>
    /// The <see cref="Enums.Ruleset"/> this <see cref="Beatmap"/> is playable on
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Count of circles
    /// </summary>
    [Column("circle_count")]
    public int CircleCount { get; set; }

    /// <summary>
    /// Count of sliders
    /// </summary>
    [Column("slider_count")]
    public int SliderCount { get; set; }

    /// <summary>
    /// Count of spinners
    /// </summary>
    [Column("spinner_count")]
    public int SpinnerCount { get; set; }

    /// <summary>
    /// Max possible combo
    /// </summary>
    [Column("max_combo")]
    public int MaxCombo { get; set; }

    /// <summary>
    /// A collection of <see cref="Game"/>s played on the <see cref="Beatmap"/>
    /// </summary>
    public ICollection<Game> Games { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="Tournament"/>s which pooled this beatmap
    /// </summary>
    public ICollection<Tournament> TournamentsPooledIn { get; set; } = [];
}
