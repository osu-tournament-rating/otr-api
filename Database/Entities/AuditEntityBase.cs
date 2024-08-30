using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Interfaces;
using Database.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Database.Entities;

/// <summary>
/// Base class for a <typeparamref name="TAudit"/> entity that serves as an audit for a <see cref="TAuditable"/> entity
/// </summary>
/// <typeparam name="TAuditable">Derived audit</typeparam>
/// <typeparam name="TAudit">Entity to be audited</typeparam>
[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public abstract class AuditEntityBase<TAuditable, TAudit> : IAuditEntity
    where TAuditable : IAuditableEntity<TAudit>
    where TAudit : IAuditEntity
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; }

    [Column("created")]
    public DateTime Created { get; }

    [Column("ref_id_lock")]
    public int ReferenceIdLock { get; set; }

    [Column("ref_id")]
    public int? ReferenceId { get; set; }

    [Column("action_user_id")]
    public int? ActionUserId { get; init; }

    [Column("action_type")]
    public AuditActionType ActionType { get; }

    [Column("changes", TypeName = "jsonb")]
    public IDictionary<string, AuditChangelogEntry> Changes { get; } = new Dictionary<string, AuditChangelogEntry>();

    public virtual void GenerateAudit(EntityEntry origEntityEntry, EntityEntry auditEntityEntry)
    {
        if (origEntityEntry.Entity is not TAuditable || auditEntityEntry.Entity is not TAudit)
        {
            return;
        }

        // No need to store a changelog for created entities
        if (ActionType is not AuditActionType.Updated)
        {
            return;
        }

        // Exclude common props like Id, Created, Updated
        IEnumerable<string> excludePropNames = typeof(IUpdateableEntity)
            .GetProperties()
            .Select(prop => prop.Name)
            .ToList();

        var newChanges = new Dictionary<string, AuditChangelogEntry>();

        foreach (PropertyEntry? prop in origEntityEntry.Properties.Where(p => p.IsModified && !excludePropNames.Contains(p.Metadata.Name)))
        {
            if (prop.OriginalValue is not null && prop.CurrentValue is not null && prop.OriginalValue != prop.CurrentValue)
            {
                newChanges.Add(
                    prop.Metadata.Name,
                    new AuditChangelogEntry { OriginalValue = prop.OriginalValue, NewValue = prop.CurrentValue }
                );
            }
        }

        // Setting the current value from the entry makes the change tracker aware of the change
        auditEntityEntry.Property(nameof(Changes)).CurrentValue = newChanges;
    }
}
