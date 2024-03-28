namespace API.Repositories.Interfaces;

public interface IRepository<T>
    where T : class
{
    // CRUD operations

    /// <summary>
    /// Adds a new entity to the database
    /// </summary>
    /// <returns>The added entity</returns>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Bulk inserts a collection of entities into the database
    /// </summary>
    /// <returns>The added entities</returns>
    Task<IEnumerable<T>> CreateAsync(IEnumerable<T> entities);

    /// <summary>
    /// Gets an entity from the database by its primary key
    /// </summary>
    /// <returns>The entity, or null if not found.</returns>
    Task<T?> GetAsync(int id);

    /// <summary>
    /// Updates an entity
    /// </summary>
    /// <returns>Number of rows affected</returns>
    Task<int> UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity from the database by its ID
    /// </summary>
    /// <returns>Primary key of the deleted entity, or null if unsuccessful</returns>
    Task<int?> DeleteAsync(int id);

    /// <summary>
    /// Returns true if an entity with the given ID exists in the database.
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Bulk inserts a collection of entities into the database.
    /// </summary>
    /// <returns>Number of rows affected</returns>
    /// <remarks>If resulting entities are required, use <see cref="CreateAsync(IEnumerable{T})"/></remarks>
    Task<int> BulkInsertAsync(IEnumerable<T> entities);

    /// <summary>
    /// Returns all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();
}
