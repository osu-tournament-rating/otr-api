using System.ComponentModel.DataAnnotations;

namespace DWS.Messages;

/// <summary>
/// Message used to request fetching player data from the osu! API.
/// </summary>
public record FetchPlayerMessage : Message
{
    /// <summary>
    /// The osu! player ID to fetch data for.
    /// </summary>
    [Required]
    public long OsuPlayerId { get; init; }
}
