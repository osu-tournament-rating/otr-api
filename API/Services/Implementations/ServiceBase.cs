using API.Services.Interfaces;

namespace API.Services.Implementations;

public class ServiceBase<T> : IService<T> where T : class
{
	private readonly ILogger _logger;

	protected ServiceBase(ILogger logger)
	{
		_logger = logger;
	}

	public virtual async Task<T> CreateAsync(T entity)
	{
		using (var context = new OtrContext())
		{
			return (await context.Set<T>().AddAsync(entity)).Entity;
		}
	}

	public virtual async Task<T?> GetAsync(int id)
	{
		using (var context = new OtrContext())
		{
			return await context.Set<T>().FindAsync(id);
		}
	}

	public virtual async Task<int> UpdateAsync(T entity)
	{
		using (var context = new OtrContext())
		{
			context.Set<T>().Update(entity);
			return await context.SaveChangesAsync();
		}
	}

	public virtual async Task<int?> DeleteAsync(int id)
	{
		using (var context = new OtrContext())
		{
			var entity = await context.Set<T>().FindAsync(id);
			if (entity == null)
			{
				return null;
			}

			context.Set<T>().Remove(entity);
			await context.SaveChangesAsync();
			return id;
		}
	}

	public virtual async Task<bool> ExistsAsync(int id)
	{
		using (var context = new OtrContext())
		{
			return await context.Set<T>().FindAsync(id) != null;
		}
	}

	public async Task<int> BulkInsertAsync(IEnumerable<T> entities)
	{
		using (var context = new OtrContext())
		{
			await context.Set<T>().AddRangeAsync(entities);
			return await context.SaveChangesAsync();
		}
	}
}