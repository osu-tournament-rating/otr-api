namespace API.DTOs;

public class UserDTO
{
    /// <summary>
    /// Id (primary key) of the user
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// List of permissions granted to the associated user
    /// </summary>
    public string[]? Scopes { get; set; }

    /// <summary>
    /// Id (primary key) of the associated player
    /// </summary>
    public int? PlayerId { get; set; }

    /// <summary>
    /// osu! account id of the associated player
    /// </summary>
    public long? OsuId { get; set; }
    public string? OsuCountry { get; set; }
    public int? OsuPlayMode { get; set; }
    public string? OsuUsername { get; set; }
}
