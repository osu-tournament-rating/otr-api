using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Entities.Interfaces;
using Database.Utilities;

namespace Database.Entities;

/// <summary>
/// Base entity
/// </summary>
public abstract class EntityBase : IEntity
{
    [Key]
    [AuditIgnore]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [AuditIgnore]
    public DateTime Created { get; init; }
}
