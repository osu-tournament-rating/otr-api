using Database.Entities.Interfaces;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Resolvers.Implementations;

public abstract class ProcessorResolver<TEntity> : IProcessorResolver<TEntity> where TEntity : class, IProcessableEntity
{
    protected readonly IEnumerable<IProcessor<TEntity>> Processors;

    protected ProcessorResolver(IEnumerable<IProcessor<TEntity>> processors)
    {
        IProcessor<TEntity>[] processorsArray = processors.ToArray();
        if (processorsArray.Length == 0)
        {
            throw new InvalidOperationException($"No processors were registered [Entity: {nameof(TEntity)}]");
        }

        Processors = processorsArray;
    }

    public IEnumerable<IProcessor<TEntity>> GetAll() => Processors.OrderBy(p => p.Order);

    public abstract IProcessor<TEntity> GetAutomationChecksProcessor();

    public abstract IProcessor<TEntity> GetVerificationProcessor();
}
