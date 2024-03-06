using API.Entities;
using API.Entities.Interfaces;

namespace API.Repositories.Interfaces;

public interface IHistoryRepository<TEntity, THistory> : IRepository<TEntity>
    where TEntity : class, IEntityBase
    where THistory : class, IHistoryEntity
{
    /// <summary>
    /// Updates <paramref name="entity"/> and blames the change on <see cref="User"/> with id <paramref name="modifierId"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="modifierId"></param>
    /// <returns></returns>
    Task<int> UpdateAsync(TEntity entity, int modifierId);

    /// <summary>
    /// Deletes <typeparamref name="TEntity"/> of <paramref name="id"/> and blames the change on <see cref="User"/> with id <paramref name="modifierId"/>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifierId"></param>
    /// <returns></returns>
    Task<int?> DeleteAsync(int id, int modifierId);
}
