namespace API.DTOs;

public class RatingDTO
{
	public int PlayerId { get; set; }
	public double Mu { get; set; }
	public double Sigma { get; set; }
	public int Mode { get; set; }
	public DateTime Created { get; set; }
}