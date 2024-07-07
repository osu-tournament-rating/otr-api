using Database.Entities.Interfaces;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Resolvers.Implementations;

public abstract class ProcessorResolver<TEntity> : IProcessorResolver<TEntity> where TEntity : class, IProcessableEntity
{
    protected readonly IEnumerable<IProcessor<TEntity>> Processors;

    protected ProcessorResolver(IEnumerable<IProcessor<TEntity>> processors)
    {
        processors = processors.ToList();
        if (!processors.Any())
        {
            throw new InvalidOperationException($"No processors were registered [Entity: {nameof(TEntity)}]");
        }

        Processors = processors;
    }

    public IEnumerable<IProcessor<TEntity>> GetAll() => Processors.OrderBy(p => p.Order);

    public abstract IProcessor<TEntity> GetAutomationChecksProcessor();
}
