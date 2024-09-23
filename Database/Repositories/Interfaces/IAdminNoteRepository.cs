using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IAdminNoteRepository<TAdminNoteEntity> : IRepository<TAdminNoteEntity> where TAdminNoteEntity : AdminNoteEntityBase
{
    /// <summary>
    /// Gets a collection of <typeparamref name="TAdminNoteEntity"/> entities by their parent reference Id.
    /// </summary>
    /// <param name="referenceId">The id of the parent entity.</param>
    /// <example>Say we wish to find all AdminNotes for a particular <see cref="Match"/>.
    /// The id of the match would be passed as the referenceId argument.</example>
    /// <returns>A collection of AdminNotes for a particular referenceId</returns>
    Task<ICollection<TAdminNoteEntity>> GetAdminNotesAsync(int referenceId);
}
