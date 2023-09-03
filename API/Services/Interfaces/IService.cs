using API.Entities.Bases;

namespace API.Services.Interfaces;

public interface IService<T> where T : class, IEntity
{
	// CRUD operations

	/// <summary>
	///  Adds a new entity to the database. If successful, returns the primary key of the entity, otherwise null.
	/// </summary>
	Task<int?> CreateAsync(T entity);

	/// <summary>
	///  Gets an entity from the database by its ID. Returns null if not found.
	/// </summary>
	Task<T?> GetAsync(int id);

	/// <summary>
	///  Updates an entity in the database by its ID. If successful, returns the primary key of the entity, otherwise null.
	/// </summary>
	Task<int?> UpdateAsync(T entity);

	/// <summary>
	///  Deletes an entity from the database by its ID. If successful, returns the primary key of the entity, otherwise null.
	/// </summary>
	Task<int?> DeleteAsync(int id);

	/// <summary>
	///  Returns true if an entity with the given ID exists in the database.
	/// </summary>
	Task<bool> ExistsAsync(int id);
}