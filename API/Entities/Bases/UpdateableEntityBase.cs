using Dapper;

namespace API.Entities.Bases;

public class UpdateableEntityBase : EntityBase, IUpdateableEntity
{
	[Column("updated")]
	public DateTime? Updated { get; set; }
}