namespace API.Entities.Interfaces;

public interface IUpdateableEntity : IEntity
{
    public DateTime? Updated { get; set; }
}
