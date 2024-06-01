using Database.Entities;
using Database.Entities.Interfaces;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IHistoryRepository<TEntity, THistory> : IRepository<TEntity>
    where TEntity : class, IEntity
    where THistory : class, IHistoryEntity
{
    /// <summary>
    /// Updates the <typeparamref name="TEntity"></typeparamref> and blames the change on <see cref="User"/> with id <paramref name="modifierId"/>
    /// </summary>
    /// <returns>Number of rows affected</returns>
    Task<int> UpdateAsync(TEntity entity, int? modifierId);

    /// <summary>
    /// Updates a list of <typeparamref name="TEntity"/> and blames the change on <see cref="User"/> with id <paramref name="modifierId"/>
    /// </summary>
    /// <returns>Number of rows affected</returns>
    Task<int> UpdateAsync(IEnumerable<TEntity> entities, int? modifierId);

    /// <summary>
    /// Deletes <typeparamref name="TEntity"/> of <paramref name="id"/> and blames the change on <see cref="User"/> with id <paramref name="modifierId"/>
    /// </summary>
    /// <returns>Number of rows affected</returns>
    Task<int?> DeleteAsync(int id, int? modifierId);
}
