namespace API.Entities.Bases;

public class UpdateableEntityBase : EntityBase, IUpdateableEntity
{
	public DateTime? Updated { get; set; }
}