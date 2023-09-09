using API.Services.Interfaces;

namespace API.Services.Implementations;

public class ServiceBase<T> : IService<T> where T : class
{
	private readonly ILogger _logger;
	private readonly OtrContext _context;

	protected ServiceBase(ILogger logger, OtrContext context)
	{
		_logger = logger;
		_context = context;
	}

	public virtual async Task<T> CreateAsync(T entity)
	{
		using (_context)
		{
			return (await _context.Set<T>().AddAsync(entity)).Entity;
		}
	}

	public virtual async Task<T?> GetAsync(int id)
	{
		using (_context)
		{
			return await _context.Set<T>().FindAsync(id);
		}
	}

	public virtual async Task<int> UpdateAsync(T entity)
	{
		using (_context)
		{
			_context.Set<T>().Update(entity);
			return await _context.SaveChangesAsync();
		}
	}

	public virtual async Task<int?> DeleteAsync(int id)
	{
		using (_context)
		{
			var entity = await _context.Set<T>().FindAsync(id);
			if (entity == null)
			{
				return null;
			}

			_context.Set<T>().Remove(entity);
			await _context.SaveChangesAsync();
			return id;
		}
	}

	public virtual async Task<bool> ExistsAsync(int id)
	{
		using (_context)
		{
			return await _context.Set<T>().FindAsync(id) != null;
		}
	}

	public async Task<int> BulkInsertAsync(IEnumerable<T> entities)
	{
		using (_context)
		{
			await _context.Set<T>().AddRangeAsync(entities);
			return await _context.SaveChangesAsync();
		}
	}
}