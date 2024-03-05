namespace API.DTOs;

public class UserInfoDTO
{
    /// <summary>
    ///  The playerId of the user
    /// </summary>
    public int? Id { get; set; }
    public int? UserId { get; set; }
    public long? OsuId { get; set; }
    public string? OsuCountry { get; set; }
    public int OsuPlayMode { get; set; }
    public string? Username { get; set; }
    public string[]? Roles { get; set; }
}
