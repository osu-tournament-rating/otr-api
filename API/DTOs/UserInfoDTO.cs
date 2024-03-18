namespace API.DTOs;

public class UserInfoDTO
{
    /// <summary>
    /// Id (primary key) of the associated user
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// List of permissions granted to the associated user
    /// </summary>
    public string[]? Scopes { get; set; }
    public long? OsuId { get; set; }
    public string? OsuCountry { get; set; }
    public int? OsuPlayMode { get; set; }
    public string? OsuUsername { get; set; }
}
