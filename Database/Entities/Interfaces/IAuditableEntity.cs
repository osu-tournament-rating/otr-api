namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces an entity that has it's changes tracked by audits
/// </summary>
/// <typeparam name="TAudit">Type of the entity that serves as the audit</typeparam>
public interface IAuditableEntity<TAudit> where TAudit : IAuditEntity
{
    /// <summary>
    /// A collection of <typeparamref name="TAudit"/>s that serve as a changelog for the entity
    /// </summary>
    public ICollection<TAudit> Audits { get; set; }
}
