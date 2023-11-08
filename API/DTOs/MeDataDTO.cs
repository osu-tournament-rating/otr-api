namespace API.DTOs;

public class MeDataDTO
{
	public int? Id { get; set; }
	public long? OsuId { get; set; }
	public string? OsuCountry { get; set; } = null!;
	public int OsuPlayMode { get; set; }
}