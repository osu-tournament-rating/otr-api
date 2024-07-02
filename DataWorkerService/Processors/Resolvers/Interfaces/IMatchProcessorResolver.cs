using Database.Entities;

namespace DataWorkerService.Processors.Resolvers.Interfaces;

/// <inheritdoc/>
public interface IMatchProcessorResolver : IProcessorResolver<Match>
{
    /// <summary>
    /// Gets the <see cref="IProcessor{TEntity}"/> implementation of the Match Data Processor
    /// </summary>
    IProcessor<Match> GetDataProcessor();
}
