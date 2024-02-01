namespace API.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
	// CRUD operations

	/// <summary>
	///  Adds a new entity to the database. Returns the added entity.
	/// </summary>
	Task<T?> CreateAsync(T entity);

	/// <summary>
	///  Gets an entity from the database by its primary key. Returns null if not found.
	/// </summary>
	Task<T?> GetAsync(int id);

	/// <summary>
	/// Updates an entity, returning the number of rows affected.
	/// </summary>
	Task<int> UpdateAsync(T entity);

	/// <summary>
	///  Deletes an entity from the database by its ID. If successful, returns the primary key of the entity, otherwise null.
	/// </summary>
	Task<int?> DeleteAsync(int id);

	/// <summary>
	///  Returns true if an entity with the given ID exists in the database.
	/// </summary>
	Task<bool> ExistsAsync(int id);

	/// <summary>
	/// Bulk inserts a collection of entities into the database.
	/// </summary>
	/// <param name="entities"></param>
	/// <returns>Number of rows affected</returns>
	Task<int> BulkInsertAsync(IEnumerable<T> entities);

	Task<IEnumerable<T>> GetAllAsync();
}