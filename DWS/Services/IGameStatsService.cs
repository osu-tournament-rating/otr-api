using Database.Entities;

namespace DWS.Services;

/// <summary>
/// Service for processing game statistics.
/// </summary>
/// <remarks>
/// Entity changes made by implementations of this service are not persisted to the database.
/// Only the tournament stats processor will persist entities.
/// </remarks>
public interface IGameStatsService
{
    /// <summary>
    /// Processes statistics for a game.
    /// </summary>
    /// <remarks>
    /// This method modifies the provided game entity, but changes are not persisted to the database.
    /// Database persistence is handled exclusively by the tournament stats processor.
    /// </remarks>
    /// <param name="game">The game to process.</param>
    Task<bool> ProcessGameStatsAsync(Game game);
}
