using Dapper;

namespace API.Entities.Bases;

public class EntityBase : IEntity
{
	[Key]
	public int Id { get; set; }
	public DateTime Created { get; set; } = DateTime.UtcNow;
}