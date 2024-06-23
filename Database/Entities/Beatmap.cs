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
    /// osu! id of the mapper
    /// </summary>
    [Column("mapper_id")]
    public long MapperId { get; init; }

    /// <summary>
    /// osu! username of the mapper
    /// </summary>
    [MaxLength(32)]
    [Column("mapper_name")]
    public string MapperName { get; init; } = null!;

    /// <summary>
    /// Song artist
    /// </summary>
    [MaxLength(512)]
    [Column("artist")]
    public string Artist { get; init; } = null!;

    /// <summary>
    /// Song title
    /// </summary>
    [MaxLength(512)]
    [Column("title")]
    public string Title { get; init; } = null!;

    /// <summary>
    /// Difficulty name
    /// </summary>
    [MaxLength(255)]
    [Column("diff_name")]
    public string DiffName { get; init; } = null!;

    /// <summary>
    /// Ranked status
    /// </summary>
    [Column("ranked_status")]
    public BeatmapRankedStatus RankedStatus { get; init; }

    /// <summary>
    /// Star rating
    /// </summary>
    [Column("sr")]
    public double Sr { get; set; }

    /// <summary>
    /// Beats per minute
    /// </summary>
    [Column("bpm")]
    public double? Bpm { get; init; }

    /// <summary>
    /// Circle size
    /// </summary>
    [Column("cs")]
    public double Cs { get; init; }

    /// <summary>
    /// Approach rate
    /// </summary>
    [Column("ar")]
    public double Ar { get; init; }

    /// <summary>
    /// Hp
    /// </summary>
    [Column("hp")]
    public double Hp { get; init; }

    /// <summary>
    /// Overall difficulty
    /// </summary>
    [Column("od")]
    public double Od { get; init; }

    /// <summary>
    /// Total length of the song
    /// </summary>
    [Column("length")]
    public double Length { get; init; }

    /// <summary>
    /// The <see cref="Enums.Ruleset"/> this <see cref="Beatmap"/> is playable on
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Count of circles
    /// </summary>
    [Column("circle_count")]
    public int CircleCount { get; init; }

    /// <summary>
    /// Count of sliders
    /// </summary>
    [Column("slider_count")]
    public int SliderCount { get; init; }

    /// <summary>
    /// Count of spinners
    /// </summary>
    [Column("spinner_count")]
    public int SpinnerCount { get; init; }

    /// <summary>
    /// Max possible combo
    /// </summary>
    [Column("max_combo")]
    public int MaxCombo { get; init; }

    /// <summary>
    /// A collection of <see cref="Game"/>s played on the <see cref="Beatmap"/>
    /// </summary>
    public ICollection<Game> Games { get; init; } = new List<Game>();
}
