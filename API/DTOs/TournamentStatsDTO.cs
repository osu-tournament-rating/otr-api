namespace API.DTOs;

public class PlayerTournamentStatsDTO
{
	public int Count1v1 { get; set; }
	public int Count2v2 { get; set; }
	public int Count3v3 { get; set; }
	public int Count4v4 { get; set; }
	public int CountOther { get; set; }
	public ICollection<TournamentDTO>? TopPerformances { get; set; }
}