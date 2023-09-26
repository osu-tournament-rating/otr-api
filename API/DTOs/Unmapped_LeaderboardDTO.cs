namespace API.DTOs;

public class Unmapped_LeaderboardDTO
{
	public int GlobalRank { get; set; }
	public string Name { get; set; }
	public string Tier { get; set; }
	public int Rating { get; set; }
	public int MatchesPlayed { get; set; }
	public double WinRate { get; set; }
}