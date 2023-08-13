using Dapper;

namespace API.Entities.Bases;

public class EntityBase : IEntity
{
	[Key]
	public int Id { get; set; }
	[ReadOnly(true)]
	public DateTime Created { get; set; }
}