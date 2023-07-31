namespace API.Entities.Bases;

public interface IUpdateableEntity : IEntity
{
	DateTime? Updated { get; set; }
}