using System.Diagnostics;
using Database.Entities.Interfaces;

namespace DataWorkerService.Processors;

/// <summary>
/// Generic implementation of a <see cref="IProcessor{TEntity}"/> that handles simple logging and measurement of elapsed time
/// </summary>
/// <param name="logger">Logger</param>
/// <typeparam name="TEntity">Type of database entity to be processed. Must implement <see cref="IProcessableEntity"/></typeparam>
public abstract class ProcessorBase<TEntity>(ILogger logger) : IProcessor<TEntity>
    where TEntity : class, IProcessableEntity
{
    public virtual async Task ProcessAsync(TEntity entity, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing started [Id: {Id}]", entity.Id);

        var stopWatch = new Stopwatch();
        stopWatch.Start();

        await OnProcessingAsync(entity, cancellationToken);

        stopWatch.Stop();
        logger.LogInformation(
            "Processing completed [Id: {Id} | Elapsed: {Elapsed:mm\\:ss\\:fff}]",
            entity.Id,
            stopWatch.Elapsed
        );

        entity.LastProcessingDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Business logic of the processor
    /// </summary>
    /// <param name="entity">Entity to be processed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected abstract Task OnProcessingAsync(TEntity entity, CancellationToken cancellationToken);
}
