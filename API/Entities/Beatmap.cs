using API.Entities.Bases;
using Dapper;
using Newtonsoft.Json;
using static API.Osu.OsuEnums;

#pragma warning disable CS8618

namespace API.Entities;

[Table("beatmaps")]
public class Beatmap : EntityBase
{
	[Column("artist")]
	public string Artist { get; set; }
	[Column("beatmap_id")]
	[JsonProperty("beatmap_id")]
	public long BeatmapId { get; set; }
	[Column("bpm")]
	public double BPM { get; set; }
	[JsonProperty("creator_id")]
	[Column("mapper_id")]
	public long MapperId { get; set; }
	[JsonProperty("creator")]
	[Column("mapper_name")]
	public string MapperName { get; set; }
	[JsonProperty("difficultyrating")]
	[Column("sr")]
	public double SR { get; set; }
	[JsonProperty("diff_aim")]
	[Column("aim_diff")]
	public double? AimDiff { get; set; } // Null in other modes
	[JsonProperty("diff_speed")]
	[Column("speed_diff")]
	public double? SpeedDiff { get; set; } // Null in other modes
	[JsonProperty("diff_size")]
	[Column("cs")]
	public double CS { get; set; }
	[JsonProperty("diff_approach")]
	[Column("ar")]
	public double AR { get; set; }
	[JsonProperty("diff_drain")]
	[Column("hp")]
	public double HP { get; set; }
	[JsonProperty("diff_overall")]
	[Column("od")]
	public double OD { get; set; }
	[JsonProperty("hit_length")]
	[Column("drain_time")]
	public double DrainTime { get; set; }
	[JsonProperty("total_length")]
	[Column("length")]
	public double Length { get; set; }
	[Column("title")]
	public string Title { get; set; }
	[JsonProperty("version")]
	[Column("diff_name")]
	public string DiffName { get; set; }
	[JsonProperty("mode")]
	[Column("game_mode")]
	public Mode GameMode { get; set; }
	[JsonProperty("count_normal")]
	[Column("circle_count")]
	public int CircleCount { get; set; }
	[JsonProperty("count_slider")]
	[Column("slider_count")]
	public int SliderCount { get; set; }
	[JsonProperty("count_spinner")]
	[Column("spinner_count")]
	public int SpinnerCount { get; set; }
	[JsonProperty("max_combo")]
	[Column("max_combo")]
	public int MaxCombo { get; set; }
}