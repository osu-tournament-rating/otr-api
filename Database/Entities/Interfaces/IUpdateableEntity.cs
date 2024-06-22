namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces an updateable entity
/// </summary>
public interface IUpdateableEntity : IEntity
{
    /// <summary>
    /// Date of the last update to the entity
    /// </summary>
    public DateTime? Updated { get; set; }
}
