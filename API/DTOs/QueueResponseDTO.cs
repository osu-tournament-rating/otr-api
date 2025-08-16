using Common.Enums;

namespace API.DTOs;

/// <summary>
/// Response data for asynchronous queue operations
/// </summary>
public class QueueResponseDTO
{
    /// <summary>
    /// Unique identifier for tracking the queued operation
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Priority level of the request in the message queue
    /// </summary>
    public MessagePriority Priority { get; set; }
}
