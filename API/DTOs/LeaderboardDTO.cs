namespace API.DTOs;

public class LeaderboardDTO
{
	public int PlayerId { get; set; }
	public long OsuId { get; set; }
	public int GlobalRank { get; set; }
	public string Name { get; set; } = null!;
	public string Tier { get; set; } = null!;
	public double Rating { get; set; }
	public int MatchesPlayed { get; set; }
	public double WinRate { get; set; }
}