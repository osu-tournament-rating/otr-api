namespace API.DTOs;

/// <summary>
/// Represents player information
/// </summary>
public class PlayerInfoDTO
{
    /// <summary>
    /// Id of the player
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// osu! id of the player
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// osu! username of the player
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// osu! country code of the player
    /// </summary>
    public string? Country { get; set; }
}
