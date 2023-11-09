namespace API.DTOs;

public class MeDataDTO
{
	public int? Id { get; set; }
	public long? OsuId { get; set; }
	public string? OsuCountry { get; set; }
	public int OsuPlayMode { get; set; }
	public string? Username { get; set; }
	public string[]? Roles { get; set; }
}