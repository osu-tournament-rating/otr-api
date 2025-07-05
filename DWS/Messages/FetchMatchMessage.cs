using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace DWS.Messages;

public record FetchMatchMessage
{
    [Required]
    public long OsuMatchId { get; init; }

    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;

    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    public MessagePriority Priority { get; init; } = MessagePriority.Normal;
}
