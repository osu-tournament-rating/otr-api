namespace API.DTOs;

public class BeatmapDTO
{
	public string Artist { get; set; } = null!;
	public long BeatmapId { get; set; }
	public double? Bpm { get; set; }
	public long MapperId { get; set; }
	public string MapperName { get; set; } = null!;
	public double Sr { get; set; }
	public double Cs { get; set; }
	public double Ar { get; set; }
	public double Hp { get; set; }
	public double Od { get; set; }
	public double DrainTime { get; set; }
	public double Length { get; set; }
	public string Title { get; set; } = null!;
	public string? DiffName { get; set; }
}