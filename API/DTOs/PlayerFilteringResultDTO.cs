using Common.Enums;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents one player's filtering result
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
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
    /// Whether the player successfully passes all
    /// conditions of the filter
    /// </summary>
    /// <example>
    /// Consider a filter which only wants to allow players
    /// whose peak rating is less than 1000.
    ///
    /// If Player A's rating is 800 and Player B's
    /// rating is 1001, this value will be true
    /// for Player A and false for Player B.
    /// </example>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// If the player failed filtering, the fail reason
    /// </summary>
    public FilteringFailReason? FailureReason { get; set; }
}
