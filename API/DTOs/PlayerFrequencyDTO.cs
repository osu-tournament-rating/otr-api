namespace API.DTOs;

/// <summary>
/// Represents a player in the context of a teammate or opponent of another player
/// </summary>
public class PlayerFrequencyDTO
{
    public PlayerCompactDTO Player { get; set; } = null!;

    /// <summary>
    /// Number of times this teammate or opponent has played with the player
    /// </summary>
    public int Frequency { get; set; }
}
