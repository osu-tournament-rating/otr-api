namespace API.DTOs;

public class Unmapped_PlayerMatchesDTO
{
	public long OsuId { get; set; }
	public ICollection<MatchDTO> Matches { get; set; } = null!;
}