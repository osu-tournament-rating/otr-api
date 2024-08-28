namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces an updateable entity
/// </summary>
public interface IUpdateableEntity : IEntity
{
    /// <summary>
    /// Timestamp for the last update
    /// </summary>
    public DateTime? Updated { get; set; }
}
