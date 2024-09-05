using Database.Entities.Interfaces;
using DataWorkerService.Processors.Resolvers.Implementations;

namespace DataWorkerService.Processors.Resolvers.Interfaces;

/// <summary>
/// Interfaces a <see cref="IProcessor{TEntity}"/> that provides registered instances of <see cref="IProcessor{TEntity}"/>
/// </summary>
/// <typeparam name="TEntity">Type of <see cref="ProcessorResolver{TEntity}"/></typeparam>
public interface IProcessorResolver<TEntity> where TEntity : class, IProcessableEntity
{
    /// <summary>
    /// Gets all processors implementing <see cref="IProcessor{TEntity}"/>
    /// </summary>
    IEnumerable<IProcessor<TEntity>> GetAll();

    /// <summary>
    /// Gets the <see cref="IProcessor{TEntity}"/> implementation of the <typeparamref name="TEntity"/> Automation Checks Processor
    /// </summary>
    IProcessor<TEntity> GetAutomationChecksProcessor();

    /// <summary>
    /// Gets the <see cref="IProcessor{TEntity}"/> implementation of the <typeparamref name="TEntity"/> Verification Processor
    /// </summary>
    IProcessor<TEntity> GetVerificationProcessor();
}
