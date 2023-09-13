using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("beatmaps")]
[Index("BeatmapId", Name = "beatmaps_beatmapid", IsUnique = true)]
public class Beatmap
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("artist")]
    [JsonProperty("artist")]
    public string Artist { get; set; } = null!;

    [Column("beatmap_id")]
    [JsonProperty("beatmap_id")]
    public long BeatmapId { get; set; }

    [Column("bpm")]
    [JsonProperty("bpm")]
    public double? Bpm { get; set; }

    [Column("mapper_id")]
    [JsonProperty("creator_id")]
    public long MapperId { get; set; }

    [Column("mapper_name")]
    [JsonProperty("creator")]
    public string MapperName { get; set; } = null!;

    [Column("sr")]
    [JsonProperty("difficultyrating")]
    public double Sr { get; set; }

    [Column("aim_diff")]
    [JsonProperty("diff_aim")]
    public double? AimDiff { get; set; }

    [Column("speed_diff")]
    [JsonProperty("diff_speed")]
    public double? SpeedDiff { get; set; }

    [Column("cs")]
    [JsonProperty("diff_size")]
    public double Cs { get; set; }

    [Column("ar")]
    [JsonProperty("diff_approach")]
    public double Ar { get; set; }

    [Column("hp")]
    [JsonProperty("diff_drain")]
    public double Hp { get; set; }

    [Column("od")]
    [JsonProperty("diff_overall")]
    public double Od { get; set; }

    [Column("drain_time")]
    [JsonProperty("hit_length")]
    public double DrainTime { get; set; }

    [Column("length")]
    [JsonProperty("total_length")]
    public double Length { get; set; }

    [Column("title")]
    [JsonProperty("title")]
    public string Title { get; set; } = null!;

    [Column("diff_name")]
    [JsonProperty("version")]
    public string? DiffName { get; set; }

    [Column("game_mode")]
    [JsonProperty("mode")]
    public int GameMode { get; set; }

    [Column("circle_count")]
    [JsonProperty("count_normal")]
    public int CircleCount { get; set; }

    [Column("slider_count")]
    [JsonProperty("count_slider")]
    public int SliderCount { get; set; }

    [Column("spinner_count")]
    [JsonProperty("count_spinner")]
    public int SpinnerCount { get; set; }

    [Column("max_combo")]
    [JsonProperty("max_combo")]
    public int? MaxCombo { get; set; }

    [Column("created", TypeName = "timestamp with time zone")]
    [JsonProperty("submit_date")] // Mapping to submit_date as the closest match
    public DateTime Created { get; set; }

    public virtual ICollection<BeatmapModSr> BeatmapModSrs { get; set; } = null!;
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}