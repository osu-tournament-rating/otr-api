using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace API.Messages;

public record FetchBeatmapMessage
{
    [Required]
    public long BeatmapId { get; init; }

    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;

    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    public MessagePriority Priority { get; init; } = MessagePriority.Normal;
}
