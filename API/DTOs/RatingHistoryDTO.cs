namespace API.DTOs;

public class RatingHistoryDTO
{
	public int PlayerId { get; set; }
	public double Mu { get; set; }
	public double Sigma { get; set; }
	public int Mode { get; set; }
	public int MatchId { get; set; }
	public DateTime Created { get; set; }
}