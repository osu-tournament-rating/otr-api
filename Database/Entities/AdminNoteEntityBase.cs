using Database.Entities.Interfaces;
using JetBrains.Annotations;

namespace Database.Entities;

/// <summary>
/// Base class for an entity that serves as an admin note for an <see cref="IAdminNotableEntity"/>
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public abstract class AdminNoteEntityBase : UpdateableEntityBase, IAdminNoteEntity
{
    public string Note { get; set; } = string.Empty;

    public int ReferenceId { get; set; }

    public int AdminUserId { get; set; }

    public User AdminUser { get; } = null!;
}
