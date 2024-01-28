namespace API.DTOs;

public class MatchDuplicateDTO
{
	public long OsuMatchId { get; set; }
	public string MatchTitle { get; set; } = string.Empty!;
	public bool? VerifiedAsDuplicate { get; set; }
	public string? VerifiedByUsername { get; set; }
	public int? VerifiedByUserId { get; set; }
}

public class MatchDuplicateCollectionDTO
{
	public int IdRoot { get; set; }
	public string MatchTitleRoot { get; set; } = string.Empty;
	public long OsuMatchIdRoot { get; set; }
	public IList<MatchDuplicateDTO> SuspectedDuplicates { get; set; } = new List<MatchDuplicateDTO>();
}