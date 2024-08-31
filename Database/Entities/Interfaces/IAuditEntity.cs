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
    /// <remarks>
    /// Stored as a concrete copy of the primary key of the original entity.
    /// This exists so that an original entity may be deleted but it's audits will still exist
    /// </remarks>
    public int ReferenceIdLock { get; }

    /// <summary>
    /// Id of the entity being audited
    /// </summary>
    /// <remarks>
    /// Used as the foreign key that navigates back to the existing original entity.
    /// If this property is null, that means the original entity has since been deleted and
    /// <see cref="ReferenceIdLock"/> should be used instead
    /// </remarks>
    public int? ReferenceId { get; }

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
    /// <remarks>Allows the implementation of custom logic for the way the audit is generated</remarks>
    public void GenerateAudit(EntityEntry origEntityEntry);
}
