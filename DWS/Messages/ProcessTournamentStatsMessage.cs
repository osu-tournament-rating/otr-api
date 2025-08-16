namespace DWS.Messages;

/// <summary>
/// Message to trigger tournament statistics processing.
/// </summary>
public record ProcessTournamentStatsMessage : Message
{
    /// <summary>
    /// The ID of the tournament to process statistics for.
    /// </summary>
    public int TournamentId { get; init; }
}
