namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces an entity
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Timestamp of creation
    /// </summary>
    public DateTime Created { get; }
}
