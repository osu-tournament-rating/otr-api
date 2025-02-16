using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Database.Entities.Interfaces;
using Database.Enums;
using Database.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Database.Entities;

/// <summary>
/// Base class for a <typeparamref name="TAudit"/> entity that serves as an audit for an auditable entity
/// </summary>
/// <typeparam name="TAuditable">Derived audit</typeparam>
/// <typeparam name="TAudit">Entity to be audited</typeparam>
[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public abstract class AuditEntityBase<TAuditable, TAudit> : IAuditEntity
    where TAuditable : IAuditableEntity<TAudit>, IEntity
    where TAudit : IAuditEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; }

    public DateTime Created { get; }

    public int ReferenceIdLock { get; private set; }

    public int? ReferenceId { get; private set; }

    public int? ActionUserId { get; set; }

    public AuditActionType ActionType { get; private set; }

    public IDictionary<string, AuditChangelogEntry> Changes { get; } = new Dictionary<string, AuditChangelogEntry>();

    public virtual bool GenerateAudit(EntityEntry origEntityEntry)
    {
        if (origEntityEntry.Entity is not TAuditable origEntity)
        {
            return false;
        }

        // Determine action type
        AuditActionType? auditAction = origEntityEntry.State switch
        {
            // TODO: Investigate auditing entities in the created state (no primary key)
            // EntityState.Added => AuditActionType.Created,
            EntityState.Modified => AuditActionType.Updated,
            EntityState.Deleted => AuditActionType.Deleted,
            _ => null
        };

        if (!auditAction.HasValue)
        {
            return false;
        }

        // Populate audit metadata and navigations
        ReferenceId = origEntity.Id;
        ReferenceIdLock = origEntity.Id;
        ActionType = auditAction.Value;

        if (origEntity.ActionBlamedOnUserId.HasValue)
        {
            ActionUserId = origEntity.ActionBlamedOnUserId.Value;
        }

        // No need to store a changelog for created or deleted entities
        if (ActionType is not AuditActionType.Updated)
        {
            return true;
        }

        // Create changelog
        foreach (PropertyEntry? prop in origEntityEntry.Properties.Where(p => p.IsModified))
        {
            PropertyInfo? declaringProp = typeof(TAuditable).GetProperty(prop.Metadata.Name);
            if (declaringProp is not null && declaringProp.GetCustomAttribute<AuditIgnoreAttribute>() is not null)
            {
                continue;
            }

            if (prop.OriginalValue is not null && prop.CurrentValue is not null && prop.OriginalValue != prop.CurrentValue)
            {
                Changes.Add(
                    prop.Metadata.Name,
                    new AuditChangelogEntry { OriginalValue = prop.OriginalValue, NewValue = prop.CurrentValue }
                );
            }
        }

        return Changes.Count > 0;
    }
}
