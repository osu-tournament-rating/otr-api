using System.ComponentModel.DataAnnotations;

namespace DWS.Messages;

/// <summary>
/// Message used to request fetching match data from the osu! API.
/// </summary>
public record FetchMatchMessage : BaseMessage
{
    /// <summary>
    /// The osu! match ID to fetch data for.
    /// </summary>
    [Required]
    public long OsuMatchId { get; init; }
}
