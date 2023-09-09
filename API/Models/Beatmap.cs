using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Table("beatmaps")]
[Index("BeatmapId", Name = "beatmaps_beatmapid", IsUnique = true)]
public partial class Beatmap
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("artist")]
    public string Artist { get; set; } = null!;

    [Column("beatmap_id")]
    public long BeatmapId { get; set; }

    [Column("bpm")]
    public double Bpm { get; set; }

    [Column("mapper_id")]
    public long MapperId { get; set; }

    [Column("mapper_name")]
    public string MapperName { get; set; } = null!;

    [Column("sr")]
    public double Sr { get; set; }

    [Column("aim_diff")]
    public double? AimDiff { get; set; }

    [Column("speed_diff")]
    public double? SpeedDiff { get; set; }

    [Column("cs")]
    public double Cs { get; set; }

    [Column("ar")]
    public double Ar { get; set; }

    [Column("hp")]
    public double Hp { get; set; }

    [Column("od")]
    public double Od { get; set; }

    [Column("drain_time")]
    public double DrainTime { get; set; }

    [Column("length")]
    public double Length { get; set; }

    [Column("title")]
    public string Title { get; set; } = null!;

    [Column("diff_name")]
    public string? DiffName { get; set; }

    [Column("game_mode")]
    public int GameMode { get; set; }

    [Column("circle_count")]
    public int CircleCount { get; set; }

    [Column("slider_count")]
    public int SliderCount { get; set; }

    [Column("spinner_count")]
    public int SpinnerCount { get; set; }

    [Column("max_combo")]
    public int MaxCombo { get; set; }

    [Column("created", TypeName = "timestamp without time zone")]
    public DateTime Created { get; set; }
}
