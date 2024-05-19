namespace API.Entities.Interfaces;

public interface IEntity
{
    /// <summary>
    /// Primary key of the entity
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Date the entity was created
    /// </summary>
    public DateTime Created { get; set; }
}
