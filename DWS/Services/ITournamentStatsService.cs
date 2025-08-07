namespace DWS.Services;

/// <summary>
/// Service for processing tournament statistics.
/// </summary>
/// <remarks>
/// Entity changes made by implementations of this service are not persisted to the database.
/// Only the tournament stats processor will persist entities.
/// </remarks>
public interface ITournamentStatsService
{
    /// <summary>
    /// Processes statistics for a tournament.
    /// </summary>
    /// <remarks>
    /// This method modifies tournament-related entities, but changes are not persisted to the database.
    /// Database persistence is handled exclusively by the tournament stats processor.
    /// </remarks>
    /// <param name="tournamentId">The ID of the tournament to process.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates if processing was successful.</returns>
    Task<bool> ProcessTournamentStatsAsync(int tournamentId);
}
