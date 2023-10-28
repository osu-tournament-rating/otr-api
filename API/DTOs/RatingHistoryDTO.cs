namespace API.DTOs;

public class RatingHistoryDTO
{
	public int MatchId { get; set; }
	public int PlayerId { get; set; }
	public double Mu { get; set; }
	public double Sigma { get; set; }
	public int Mode { get; set; }
	public long OsuMatchId { get; set; }
	public string? TournamentName { get; set; }
	public string? MatchName { get; set; }
	public string? Abbreviation { get; set; }
	public DateTime Created { get; set; }
	public int MuCasted => (int) Mu;
	public int SigmaCasted => (int) Sigma;
}