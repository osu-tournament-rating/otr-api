namespace API.DTOs;

public class GameDTO
{
	public int Id { get; set; }
	public int PlayMode { get; set; }
	public int ScoringType { get; set; }
	public int TeamType { get; set; }
	public int Mods { get; set; }
	public long GameId { get; set; }
	public double PostModSr { get; set; }
	public DateTime StartTime { get; set; }
	public DateTime? EndTime { get; set; }
	public BeatmapDTO? Beatmap { get; set; }
	public List<MatchScoreDTO> MatchScores { get; set; } = new();
}