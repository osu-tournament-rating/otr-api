namespace API.DTOs;

public class PlayerInfoDTO
{
    public int Id { get; set; }
    public long OsuId { get; set; }
    public string? Username { get; set; }
    public string? Country { get; set; }
}
