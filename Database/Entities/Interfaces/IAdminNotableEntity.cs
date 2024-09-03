namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces an entity that stores admin notes
/// </summary>
public interface IAdminNotableEntity<TAdminNote> where TAdminNote : IAdminNoteEntity
{
    /// <summary>
    /// A collection of <typeparamref name="TAdminNote"/> containing relevant information
    /// noted by admin users
    /// </summary>
    public ICollection<TAdminNote> AdminNotes { get; set; }
}
