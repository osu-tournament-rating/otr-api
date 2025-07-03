using System.ComponentModel.DataAnnotations;

namespace API.Messages;

public record FetchBeatmapMessage
{
    [Required]
    public long BeatmapId { get; init; }

    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;

    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
