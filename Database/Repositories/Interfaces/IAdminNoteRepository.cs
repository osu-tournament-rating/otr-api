using Database.Entities;
using Database.Entities.Interfaces;

namespace Database.Repositories.Interfaces;

public interface IAdminNoteRepository
{
    Task<bool> ExistsAsync<TAdminNote>(int id) where TAdminNote : AdminNoteEntityBase;

    /// <summary>
    /// Creates a <typeparamref name="TAdminNote"/>
    /// </summary>
    /// <param name="entity">The <typeparamref name="TAdminNote"/> to be created</param>
    /// <typeparam name="TAdminNote">The type of admin note being created</typeparam>
    Task<TAdminNote> CreateAsync<TAdminNote>(TAdminNote entity) where TAdminNote : AdminNoteEntityBase;

    /// <summary>
    /// Gets a <typeparamref name="TAdminNote"/>
    /// </summary>
    /// <param name="id">Id of the <typeparamref name="TAdminNote"/></param>
    /// <typeparam name="TAdminNote">The type of admin note being retrieved</typeparam>
    /// <returns>The <typeparamref name="TAdminNote"/>, or null if not found</returns>
    /// <remarks>
    /// Includes the <see cref="AdminNoteEntityBase.AdminUser"/> and <see cref="User.Player"/>.
    /// Returned entities are tracked by the context
    /// </remarks>
    Task<TAdminNote?> GetAsync<TAdminNote>(int id) where TAdminNote : AdminNoteEntityBase;

    /// <summary>
    /// Gets a collection of <typeparamref name="TAdminNote"/> entities by their parent reference Id.
    /// </summary>
    /// <param name="referenceId">Id of the parent entity.</param>
    /// <typeparam name="TAdminNote">The type of admin note being retrieved</typeparam>
    /// <returns>A collection of <typeparamref name="TAdminNote"/>s for the given referenceId</returns>
    /// <remarks>
    /// Includes the <see cref="AdminNoteEntityBase.AdminUser"/> and <see cref="User.Player"/> for each
    /// <typeparamref name="TAdminNote"/>. Returned entities are not tracked by the context
    /// </remarks>
    Task<IEnumerable<TAdminNote>> ListAsync<TAdminNote>(int referenceId) where TAdminNote : AdminNoteEntityBase;

    /// <summary>
    /// Updates a <typeparamref name="TAdminNote"/>
    /// </summary>
    /// <param name="entity">The <typeparamref name="TAdminNote"/> to be updated</param>
    /// <typeparam name="TAdminNote">The type of admin note being updated</typeparam>
    /// <returns>The updated <typeparamref name="TAdminNote"/></returns>
    Task<TAdminNote> UpdateAsync<TAdminNote>(TAdminNote entity) where TAdminNote : AdminNoteEntityBase;

    /// <summary>
    /// Deletes a <typeparamref name="TAdminNote"/>
    /// </summary>
    /// <param name="entity">The <typeparamref name="TAdminNote"/> to be deleted</param>
    /// <typeparam name="TAdminNote">The type of admin note being deleted</typeparam>
    Task DeleteAsync<TAdminNote>(TAdminNote entity) where TAdminNote : AdminNoteEntityBase;
}
