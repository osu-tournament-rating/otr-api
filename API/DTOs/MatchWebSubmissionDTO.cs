namespace API.DTOs;

public class MatchWebSubmissionDTO
{
	public string TournamentName { get; set; } = null!;
	public string Abbreviation { get; set; } = null!;
	public string ForumPost { get; set; } = null!;
	public int RankRangeLowerBound { get; set; }
	public int TeamSize { get; set; }
	public int Mode { get; set; }
	public int SubmitterId { get; set; }
	public IEnumerable<long> Ids { get; set; } = new List<long>();
}