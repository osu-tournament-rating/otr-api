using API.Entities.Bases;
using Dapper;

#pragma warning disable CS8618

namespace API.Entities;

[Table("beatmap")]
public class Beatmap : EntityBase
{
    [Column("artist")]
    public string Artist { get; set; }
    
    [Column("beatmap_id")]
    public int BeatmapId { get; set; }
    
    [Column("bpm")]
    public int Bpm { get; set; }
    
    [Column("mapper_id")]
    public int MapperId { get; set; }
    
    [Column("mapper_name")]
    public string  MapperName { get; set; }
    
    [Column("sr")]
    public double Sr { get; set; }
    
    [Column("aim_diff")]
    public double AimDiff { get; set; }
    
    [Column("speed_diff")]
    public double SpeedDiff { get; set; }
    
    [Column("cs")]
    public double Cs { get; set; }
    
    [Column("ar")]
    public double Ar { get; set; }
    
    [Column("hp")]
    public double Hp { get; set; }
    
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