using System.ComponentModel.DataAnnotations.Schema;
using Database.Entities.Interfaces;

namespace Database.Entities;

/// <summary>
/// Base class for an entity that serves as an admin note for an <see cref="IAdminNotableEntity"/>
/// </summary>
public abstract class AdminNoteEntityBase : UpdateableEntityBase, IAdminNoteEntity
{
    [Column("note")]
    public string Note { get; set; } = string.Empty;

    [Column("ref_id")]
    public int ReferenceId { get; set; }

    [Column("admin_user_id")]
    public int AdminUserId { get; set; }

    public User AdminUser { get; } = null!;
}
