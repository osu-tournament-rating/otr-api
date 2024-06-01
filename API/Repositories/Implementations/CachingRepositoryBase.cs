using API.Repositories.Interfaces;
using Database;
using Database.Repositories.Implementations;

namespace API.Repositories.Implementations;

/// <summary>
/// Repository base for repositories that require interfacing with the redis cache
/// </summary>
/// <param name="context">Database context</param>
/// <typeparam name="T">Database entity type</typeparam>
public class CachingRepositoryBase<T>(OtrContext context) : RepositoryBase<T>(context)
    where T : class
{
    public override async Task<T> CreateAsync(T entity)
    {
        T created = await base.CreateAsync(entity);
        await TryInvalidateCacheAsync();
        return created;
    }

    public override async Task<IEnumerable<T>> CreateAsync(IEnumerable<T> entities)
    {
        IEnumerable<T> created = await base.CreateAsync(entities);
        await TryInvalidateCacheAsync();
        return created;
    }

    public override async Task<int> UpdateAsync(T entity)
    {
        var result = await base.UpdateAsync(entity);
        await TryInvalidateCacheAsync();
        return result;
    }

    public override async Task<int> UpdateAsync(IEnumerable<T> entities)
    {
        var result = await base.UpdateAsync(entities);
        await TryInvalidateCacheAsync();
        return result;
    }

    public override async Task<int?> DeleteAsync(int id)
    {
        var result = await base.DeleteAsync(id);
        await TryInvalidateCacheAsync();
        return result;
    }

    public override async Task<int> BulkInsertAsync(IEnumerable<T> entities)
    {
        var result = await base.BulkInsertAsync(entities);
        await TryInvalidateCacheAsync();
        return result;
    }

    /// <summary>
    /// For repositories implementing <see cref="API.Repositories.Interfaces.IUsesCache"/>, invalidates entries on CRUD actions
    /// </summary>
    private async Task TryInvalidateCacheAsync()
    {
        if (this is IUsesCache repository)
        {
            await repository.InvalidateCacheEntriesAsync();
        }
    }
}
