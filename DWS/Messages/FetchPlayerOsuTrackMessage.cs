using System.ComponentModel.DataAnnotations;

namespace DWS.Messages;

/// <summary>
/// Message used to request fetching player historical stats data from the osu!track API.
/// </summary>
public record FetchPlayerOsuTrackMessage : Message
{
    /// <summary>
    /// The osu! player ID to fetch osu!track data for.
    /// </summary>
    [Required]
    public long OsuPlayerId { get; init; }
}
