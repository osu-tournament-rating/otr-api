using API.Entities.Bases;
using Dapper;
using Newtonsoft.Json;

#pragma warning disable CS8618

namespace API.Entities;

[Table("beatmap")]
public class Beatmap : EntityBase
{
	[Column("artist")]
	public string Artist { get; set; }
	[Column("beatmap_id")]
	public long BeatmapId { get; set; }
	[Column("bpm")]
	public double BPM { get; set; }
	[Column("mapper_id")]
	public int MapperId { get; set; }
	[Column("mapper_name")]
	public string MapperName { get; set; }
	[Column("sr")]
	public double SR { get; set; }
	[Column("aim_diff")]
	public double AimDiff { get; set; }
	[Column("speed_diff")]
	public double SpeedDiff { get; set; }
	[Column("cs")]
	public double CS { get; set; }
	[Column("ar")]
	public double AR { get; set; }
	[Column("hp")]
	public double HP { get; set; }
	[Column("drain_time")]
	public double DrainTime { get; set; }
	[Column("length")]
	public double Length { get; set; }
	[Column("title")]
	public string Title { get; set; }
	[Column("diff_name")]
	public string DiffName { get; set; }
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
}