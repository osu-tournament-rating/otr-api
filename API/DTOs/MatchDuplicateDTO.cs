namespace API.DTOs;

public class MatchDuplicateDTO
{
	public long OsuMatchId { get; set; }
	public string Name { get; set; } = string.Empty;
	public bool? VerifiedAsDuplicate { get; set; }
	public string? VerifiedByUsername { get; set; }
	public int? VerifiedByUserId { get; set; }
}