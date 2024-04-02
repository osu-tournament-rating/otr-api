using API.Enums;

namespace API.DTOs;

/// <summary>
/// Represents one player's screening result
/// </summary>
public class PlayerScreeningResultDTO
{
    /// <summary>
    /// The id of the player, if found
    /// </summary>
    public int? PlayerId { get; set; }
    /// <summary>
    /// The username of the player, if found
    /// </summary>
    public string? Username { get; set; }
    /// <summary>
    /// The osu! id of the player
    /// </summary>
    public long OsuPlayerId { get; set; }
    /// <summary>
    /// The screening result
    /// </summary>
    public ScreeningResult ScreeningResult { get; set; }
    /// <summary>
    /// If the user failed screening, the fail reason
    /// </summary>
    public ScreeningFailReason? ScreeningFailReason { get; set; }
    /// <summary>
    /// The <see cref="ScreeningResult"/> in string form
    /// </summary>
    public string? ScreeningResultMessage => ScreeningResult.ToString();
    /// <summary>
    /// The <see cref="ScreeningFailReason"/> in string form
    /// </summary>
    public string? ScreeningFailReasonMessage => ScreeningFailReason.ToString();
}
