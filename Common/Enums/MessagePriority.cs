namespace Common.Enums;

/// <summary>
/// Represents the priority level for message queue processing.
/// Higher values indicate higher priority and will be processed first.
/// Based on RabbitMQ's 0-10 priority scale.
/// </summary>
public enum MessagePriority : byte
{
    /// <summary>
    /// Low priority messages (processed after Normal and High)
    /// </summary>
    Low = 0,

    /// <summary>
    /// Normal priority messages (default priority level)
    /// </summary>
    Normal = 5,

    /// <summary>
    /// High priority messages (processed first)
    /// </summary>
    High = 10
}
