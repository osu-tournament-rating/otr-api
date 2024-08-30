namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces an entity that has it's changes tracked by audits
/// </summary>
/// <typeparam name="TAudit">Type of the entity that serves as the audit</typeparam>
public interface IAuditableEntity<TAudit> where TAudit : IAuditEntity
{
    /// <summary>
    /// Id of the <see cref="User"/> that made an auditable action against the entity
    /// </summary>
    /// <remarks>
    /// **This property is NOT meant to be mapped into the database!**
    /// It is meant to be used on a per-scope basis to identify any user making changes to the entity and
    /// is used as metadata information for the audits themselves
    /// </remarks>
    public int? ActionBlamedOnUserId { get; set; }

    /// <summary>
    /// A collection of <typeparamref name="TAudit"/> that serve as a changelog for the entity
    /// </summary>
    public ICollection<TAudit> Audits { get; set; }
}
