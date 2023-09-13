using API.Services.Interfaces;

namespace API.Services.Implementations;

public class ServiceBase<T> : IService<T> where T : class
{
	private readonly OtrContext _context;
	private readonly ILogger _logger;

	protected ServiceBase(ILogger logger, OtrContext context)
	{
		_logger = logger;
		_context = context;
	}

	public virtual async Task<T> CreateAsync(T entity)
	{
		var created = (await _context.Set<T>().AddAsync(entity)).Entity;
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
		var entity = await _context.Set<T>().FindAsync(id);
		if (entity == null)
		{
			return null;
		}

		_context.Set<T>().Remove(entity);
		await _context.SaveChangesAsync();
		return id;
	}

	public virtual async Task<bool> ExistsAsync(int id) => await _context.Set<T>().FindAsync(id) != null;

	public async Task<int> BulkInsertAsync(IEnumerable<T> entities)
	{
		await _context.Set<T>().AddRangeAsync(entities);
		return await _context.SaveChangesAsync();
	}
}