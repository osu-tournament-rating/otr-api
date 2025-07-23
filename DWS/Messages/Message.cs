using Common.Enums;

namespace DWS.Messages;

/// <summary>
/// Base message for all DWS queue messages with common tracking and priority fields.
/// </summary>
public abstract record Message
{
    /// <summary>
    /// The timestamp when this request was created.
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
