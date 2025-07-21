using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace DWS.Messages;

/// <summary>
/// Message used to request fetching player data from the osu! API
/// </summary>
public record FetchPlayerMessage
{
    [Required]
    public long OsuPlayerId { get; init; }

    /// <summary>
    /// The timestamp when this fetch request was created.
    /// </summary>
    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Unique identifier for tracking this message through the system.
    /// </summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// The priority level for processing this message.
    /// </summary>
    public MessagePriority Priority { get; init; } = MessagePriority.Normal;
}
