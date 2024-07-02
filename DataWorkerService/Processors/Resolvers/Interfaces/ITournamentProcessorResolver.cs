using Database.Entities;

namespace DataWorkerService.Processors.Resolvers.Interfaces;

/// <inheritdoc/>
public interface ITournamentProcessorResolver : IProcessorResolver<Tournament>
{
    /// <summary>
    /// Gets the <see cref="IProcessor{TEntity}"/> implementation of the Tournament Data Processor
    /// </summary>
    IProcessor<Tournament> GetDataProcessor();
}
