using Database.Entities;

namespace DWS.Services;

/// <summary>
/// Service for processing match statistics.
/// </summary>
/// <remarks>
/// Entity changes made by implementations of this service are not persisted to the database.
/// Only the tournament stats processor will persist entities.
/// </remarks>
public interface IMatchStatsService
{
    /// <summary>
    /// Processes statistics for a match.
    /// </summary>
    /// <remarks>
    /// This method modifies the provided match entity, but changes are not persisted to the database.
    /// Database persistence is handled exclusively by the tournament stats processor.
    /// </remarks>
    /// <param name="match">The match to process.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates if processing was successful.</returns>
    Task<bool> ProcessMatchStatsAsync(Match match);
}
