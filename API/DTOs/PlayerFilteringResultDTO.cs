using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Represents one player's filtering result
/// </summary>
public class PlayerFilteringResultDTO
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
    public long OsuId { get; set; }
    /// <summary>
    /// The filtering result
    /// </summary>
    public FilteringResult FilteringResult { get; set; }
    /// <summary>
    /// If the user failed filtering, the fail reason
    /// </summary>
    public FilteringFailReason? FilteringFailReason { get; set; }
    /// <summary>
    /// The <see cref="FilteringResult"/> in string form
    /// </summary>
    public string? FilteringResultMessage => FilteringResult.ToString();
    /// <summary>
    /// The <see cref="FilteringFailReason"/> in string form
    /// </summary>
    public string? FilteringFailReasonMessage => FilteringFailReason.ToString();
}
