using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class RepositoryBase<T> : IRepository<T>
    where T : class
{
    private readonly OtrContext _context;

    protected RepositoryBase(OtrContext context)
    {
        _context = context;
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        T? created = (await _context.Set<T>().AddAsync(entity)).Entity ?? throw new Exception($"Failed to create {nameof(T)} entity");
        await _context.SaveChangesAsync();

        return created;
    }

    public virtual async Task<T?> GetAsync(int id) => await _context.Set<T>().FindAsync(id);

    public virtual async Task<int> UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<int?> DeleteAsync(int id)
    {
        T? entity = await _context.Set<T>().FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return id;
    }

    public virtual async Task<bool> ExistsAsync(int id) => await _context.Set<T>().FindAsync(id) != null;

    public virtual async Task<int> BulkInsertAsync(IEnumerable<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
}
