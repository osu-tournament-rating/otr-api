namespace API.DTOs;

public class PlayerDTO
{
    public int Id { get; set; }
    public long OsuId { get; set; }
    public string? Username { get; set; }
    public string? Country { get; set; }
}
