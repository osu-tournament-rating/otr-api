using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Entities.Interfaces;

namespace Database.Entities;

/// <summary>
/// Base entity
/// </summary>
public abstract class EntityBase : IEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("created")]
    public DateTime Created { get; set; }
}
