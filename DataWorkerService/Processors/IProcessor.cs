using Database.Entities.Interfaces;

namespace DataWorkerService.Processors;

/// <summary>
/// Interfaces a processor that runs a processing task against a <typeparamref name="TEntity"/>
/// </summary>
/// <typeparam name="TEntity">Type of database entity to be processed</typeparam>
public interface IProcessor<TEntity> where TEntity : class, IProcessableEntity
{
    /// <summary>
    /// Processes the given entity
    /// </summary>
    /// <param name="entity">Entity to be processed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task ProcessAsync(TEntity entity, CancellationToken cancellationToken);
}
