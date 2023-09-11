namespace API.DTOs;

public class Unmapped_PlayerRatingDTO
{
	public long OsuId { get; set; }
	public string Username { get; set; } = null!;
	public double Mu { get; set; }
	public double Sigma { get; set; }
}