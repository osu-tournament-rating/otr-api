namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces an entity that serves as a note about another entity created by an admin user
/// </summary>
public interface IAdminNoteEntity : IUpdateableEntity
{
    /// <summary>
    /// Message
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// Id of the admin <see cref="User"/> that created the note
    /// </summary>
    public int AdminUserId { get; set; }

    /// <summary>
    /// The admin <see cref="User"/> that created the note
    /// </summary>
    public User AdminUser { get; }
}
