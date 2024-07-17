using Database.Entities;

namespace DataWorkerService.Processors.Resolvers.Interfaces;

/// <inheritdoc/>
public interface IGameProcessorResolver : IProcessorResolver<Game>
{
    /// <summary>
    /// Gets the <see cref="IProcessor{TEntity}"/> implementation of the Game Stats Processor
    /// </summary>
    public IProcessor<Game> GetStatsProcessor();
}
