namespace API.DTOs;

public class PlayerRatingDTO
{
	public long OsuId { get; set; }
	public string Username { get; set; } = null!;
	public double Mu { get; set; }
	public double Sigma { get; set; }
}