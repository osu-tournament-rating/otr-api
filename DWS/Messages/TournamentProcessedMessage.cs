namespace DWS.Messages;

/// <summary>
/// Message sent when a tournament has been processed by the otr-processor.
/// </summary>
public record TournamentProcessedMessage : Message
{
    /// <summary>
    /// The ID of the tournament that was processed.
    /// </summary>
    public int TournamentId { get; init; }

    /// <summary>
    /// The timestamp when the tournament was processed.
    /// </summary>
    public DateTime ProcessedAt { get; init; }

    /// <summary>
    /// The action that was performed during processing (e.g., "Created", "Updated", "Skipped").
    /// </summary>
    public string Action { get; init; } = string.Empty;
}
