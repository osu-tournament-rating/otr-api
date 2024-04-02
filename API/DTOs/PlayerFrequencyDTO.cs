namespace API.DTOs;

/// <summary>
/// Represents a player in the context of a teammate or opponent of another player
/// </summary>
public class PlayerFrequencyDTO
{
    /// <summary>
    /// Id of the teammate or opponent
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// osu! id of the teammate or opponent
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// osu! username of the teammate or opponent
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Number of times this teammate or opponent has played with the player
    /// </summary>
    public int Frequency { get; set; }
}
