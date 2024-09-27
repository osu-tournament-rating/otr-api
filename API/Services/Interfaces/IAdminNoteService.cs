using API.DTOs;
using Database.Entities;

namespace API.Services.Interfaces;

public interface IAdminNoteService
{
    /// <summary>
    /// Creates a <typeparamref name="TAdminNote"/>
    /// </summary>
    /// <param name="referenceId">Id of the parent entity</param>
    /// <param name="adminUserId">Id of the admin <see cref="User"/> creating the <typeparamref name="TAdminNote"/></param>
    /// <param name="note">Content of the <typeparamref name="TAdminNote"/></param>
    /// <typeparam name="TAdminNote">The type of admin note being created</typeparam>
    /// <returns>
    /// The created <see cref="AdminNoteDTO"/> if successful, or null if a <see cref="User"/> for the
    /// given <paramref name="adminUserId"/> does not exist
    /// </returns>
    /// <remarks>
    /// This method checks for existence of the admin <see cref="User"/>, but checking for existence of the parent
    /// entity should be handled by the caller
    /// </remarks>
    Task<AdminNoteDTO?> CreateAsync<TAdminNote>(int referenceId, int adminUserId, string note)
        where TAdminNote : AdminNoteEntityBase, new();

    /// <summary>
    /// Gets an <see cref="AdminNoteDTO"/>
    /// </summary>
    /// <param name="id">Id of the <typeparamref name="TAdminNote"/></param>
    /// <typeparam name="TAdminNote">The type of admin note being retrieved</typeparam>
    /// <returns>The <typeparamref name="AdminNoteDTO"/>, or null if not found</returns>
    Task<AdminNoteDTO?> GetAsync<TAdminNote>(int id) where TAdminNote : AdminNoteEntityBase;

    /// <summary>
    /// Gets a collection of <see cref="AdminNoteDTO"/>s by their parent reference Id.
    /// </summary>
    /// <param name="referenceId">Id of the parent entity</param>
    /// <typeparam name="TAdminNote">The type of admin note being retrieved</typeparam>
    /// <returns>A collection of <see cref="AdminNoteDTO"/>s for the given referenceId</returns>
    Task<IEnumerable<AdminNoteDTO>> ListAsync<TAdminNote>(int referenceId) where TAdminNote : AdminNoteEntityBase;

    /// <summary>
    /// Updates the <see cref="AdminNoteEntityBase.Note"/> of a <typeparamref name="TAdminNote"/>
    /// </summary>
    /// <param name="id">Id of the <typeparamref name="TAdminNote"/></param>
    /// <param name="note">The new note</param>
    /// <typeparam name="TAdminNote">The type of admin note being updated</typeparam>
    /// <returns>The updated <see cref="AdminNoteDTO"/>, or null if not found</returns>
    Task<AdminNoteDTO?> UpdateAsync<TAdminNote>(int id, string note) where TAdminNote : AdminNoteEntityBase;

    /// <summary>
    /// Deletes a <typeparamref name="TAdminNote"/>
    /// </summary>
    /// <param name="id">Id of the <typeparamref name="TAdminNote"/></param>
    /// <typeparam name="TAdminNote">The type of admin note being deleted</typeparam>
    /// <returns>
    /// True if successful, false if a <typeparamref name="TAdminNote"/> for the given id does not exist
    /// </returns>
    Task<bool> DeleteAsync<TAdminNote>(int id) where TAdminNote : AdminNoteEntityBase;
}
