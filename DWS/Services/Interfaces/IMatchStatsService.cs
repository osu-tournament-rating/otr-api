using Database.Entities;

namespace DWS.Services.Interfaces;

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
    Task<bool> ProcessMatchStatsAsync(Match match);
}
