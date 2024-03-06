namespace API.Entities.Interfaces;

public interface IEntityBase
{
    public int Id { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime Created { get; set; }
}
