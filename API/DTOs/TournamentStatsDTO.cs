namespace API.DTOs;

public class PlayerTournamentStatsDTO
{
	public PlayerTournamentTeamSizeCountDTO TeamSizeCounts { get; set; } = new();
	public IEnumerable<PlayerTournamentMatchCostDTO> BestPerformances { get; set; } = new List<PlayerTournamentMatchCostDTO>();
	public IEnumerable<PlayerTournamentMatchCostDTO> WorstPerformances { get; set; } = new List<PlayerTournamentMatchCostDTO>();
}