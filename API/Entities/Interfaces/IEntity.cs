namespace API.Entities.Interfaces;

public interface IEntity
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
}
