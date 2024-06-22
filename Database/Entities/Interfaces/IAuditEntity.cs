using Database.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces an entity serving as an audit for an <see cref="IAuditableEntity{TAudit}"/>
/// </summary>
public interface IAuditEntity : IEntity
{
    /// <summary>
    /// Id of the entity being audited
    /// </summary>
    public int ReferenceId { get; set; }

    /// <summary>
    /// Id of the <see cref="User"/> that took action on the record
    /// </summary>
    public int? ActionUserId { get; }

    /// <summary>
    /// The type of action taken on the entity being audited
    /// </summary>
    public AuditActionType ActionType { get; }

    /// <summary>
    /// A dictionary containing information about property changes.
    /// The key is the name of the property, and the value is a <see cref="AuditChangelogEntry"/>
    /// </summary>
    public IDictionary<string, AuditChangelogEntry> Changes { get; }

    /// <summary>
    /// Populates the audit with values from the given <see cref="EntityEntry"/>
    /// </summary>
    /// <param name="origEntityEntry">The <see cref="EntityEntry"/> for the entity being audited</param>
    /// <param name="auditEntityEntry">The <see cref="EntityEntry"/> for the audit entity</param>
    /// <remarks>Allows the implementation of custom logic for the way the audit is generated</remarks>
    public void GenerateAudit(EntityEntry origEntityEntry, EntityEntry auditEntityEntry);
}
