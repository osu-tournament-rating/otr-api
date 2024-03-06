using API.Enums;

namespace API.Repositories.Interfaces;

public interface IHistoryRepository<THistory, TEntity> : IRepository<THistory>
    where THistory : class
{
    /// <summary>
    /// Creates a new entity of type THistory, mapped to the values of <paramref name="entity"/> with action type <paramref name="action"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<THistory?> CreateAsync(TEntity entity, HistoryActionType action);
}
