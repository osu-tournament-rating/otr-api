using System.ComponentModel.DataAnnotations.Schema;
using Database.Entities.Interfaces;
using Database.Utilities;

namespace Database.Entities;

/// <summary>
/// Base updateable entity
/// </summary>
public abstract class UpdateableEntityBase : EntityBase, IUpdateableEntity
{
    [AuditIgnore]
    public DateTime? Updated { get; set; }
}
