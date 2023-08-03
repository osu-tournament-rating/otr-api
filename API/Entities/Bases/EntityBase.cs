using Dapper;

namespace API.Entities.Bases;

public class EntityBase : IEntity
{
	[Key]
	[Column("id")]
	public int Id { get; set; }
	// TODO: Set this attribute after inserting all match data. [ReadOnly(true)]
	[Column("created")]
	public DateTime Created { get; set; }
}