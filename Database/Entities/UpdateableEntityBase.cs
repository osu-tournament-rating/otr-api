using System.ComponentModel.DataAnnotations.Schema;
using Database.Entities.Interfaces;

namespace Database.Entities;

/// <summary>
/// Base updateable entity
/// </summary>
public abstract class UpdateableEntityBase : EntityBase, IUpdateableEntity
{
    [Column("updated")]
    public DateTime? Updated { get; set; }
}
