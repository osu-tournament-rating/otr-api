namespace DWS.Services;

/// <summary>
/// Base interface for automation check services.
/// </summary>
/// <typeparam name="TEntity">The entity type to process automation checks for</typeparam>
public interface IAutomationCheckService<TEntity>
{
    /// <summary>
    /// Processes automation checks for the specified entity and updates its verification status.
    /// </summary>
    /// <param name="entityId">The ID of the entity to process</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the entity passed all automation checks, false otherwise</returns>
    Task<bool> ProcessAutomationChecksAsync(int entityId, CancellationToken cancellationToken = default);
}
