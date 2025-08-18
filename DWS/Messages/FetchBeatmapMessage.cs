using System.ComponentModel.DataAnnotations;

namespace DWS.Messages;

/// <summary>
/// Message used to request fetching beatmap data from the osu! API.
/// </summary>
public record FetchBeatmapMessage : Message
{
    /// <summary>
    /// The osu! beatmap ID to fetch data for.
    /// </summary>
    [Required]
    public long BeatmapId { get; init; }
}
