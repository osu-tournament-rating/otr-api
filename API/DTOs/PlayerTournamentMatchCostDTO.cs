namespace API.DTOs;

public class PlayerTournamentMatchCostDTO
{
	public int PlayerId { get; set; }
	public int TournamentId { get; set; }
	public string TournamentName { get; set; } = null!;
	public string TournamentAcronym { get; set; } = null!;
	public int Mode { get; set; }
	public double MatchCost { get; set; }
}