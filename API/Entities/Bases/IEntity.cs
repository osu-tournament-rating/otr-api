namespace API.Entities.Bases;

public interface IEntity
{
	int Id { get; set; }
	DateTime Created { get; set; }
}