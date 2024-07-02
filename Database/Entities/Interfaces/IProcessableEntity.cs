namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces an entity that is processed by the Data Worker Service
/// </summary>
public interface IProcessableEntity : IEntity
{
    /// <summary>
    /// Timestamp of the last time the entity was processed
    /// </summary>
    public DateTime LastProcessingDate { get; set; }
}
