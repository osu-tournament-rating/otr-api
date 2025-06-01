using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Common.Enums;
using Database.Entities.Interfaces;
using Database.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Database.Entities;

/// <summary>
/// Base class for a <typeparamref name="TAudit"/> entity that serves as an audit for an auditable entity
/// </summary>
/// <typeparam name="TEntity">Entity that is being audited</typeparam>
/// <typeparam name="TAudit">Entity to be audited</typeparam>
[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public abstract class AuditEntityBase<TEntity, TAudit> : IAuditEntity
    where TAudit : IAuditEntity
    where TEntity : IEntity
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; }

    public DateTime Created { get; } = DateTime.UtcNow;

    public int ReferenceIdLock { get; private set; }

    public int? ReferenceId { get; private set; }

    public int? ActionUserId { get; set; }

    public AuditActionType ActionType { get; private set; }

    public string? Changes { get; private set; }

    public virtual bool GenerateAudit(EntityEntry origEntityEntry, IHttpContextAccessor? httpContextAccessor)
    {
        if (origEntityEntry.Entity is not IEntity originalEntity)
        {
            return false;
        }

        // Determine action type
        AuditActionType? auditAction = origEntityEntry.State switch
        {
            EntityState.Modified => AuditActionType.Updated,
            EntityState.Deleted => AuditActionType.Deleted,
            _ => null
        };

        if (!auditAction.HasValue)
        {
            return false;
        }

        // Populate audit metadata and navigations
        ReferenceId = originalEntity.Id;
        ReferenceIdLock = originalEntity.Id;
        ActionType = auditAction.Value;

        // Set ActionUserId from HttpContext
        if (httpContextAccessor?.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                ActionUserId = userId;
            }
        }

        if (ActionType == AuditActionType.Deleted)
        {
            // For deleted entities, set Changes to null
            Changes = null;
            return true;
        }

        if (ActionType == AuditActionType.Updated)
        {
            var changes = new Dictionary<string, object>();

            // For updated entities, only capture changed properties
            var changedProperties = origEntityEntry.Properties
                .Where(p => p.IsModified && !Equals(p.CurrentValue, p.OriginalValue) && !ShouldIgnoreProperty(p))
                .ToList();

            if (changedProperties.Count == 0)
            {
                // Check for collection and reference changes
                bool hasCollectionChanges = origEntityEntry.Collections.Any(c => c.IsModified);
                bool hasReferenceChanges = origEntityEntry.References.Any(r => r.IsModified);

                if (!hasCollectionChanges && !hasReferenceChanges)
                {
                    return false; // No actual changes to log
                }
            }

            foreach (var property in changedProperties)
            {
                changes[property.Metadata.Name] = new
                {
                    NewValue = property.CurrentValue,
                    property.OriginalValue
                };
            }

            if (changes.Count == 0)
            {
                return false; // No changes to audit
            }

            Changes = JsonSerializer.Serialize(changes, SerializerOptions);
            return true;
        }

        return false;
    }

    private static bool ShouldIgnoreProperty(PropertyEntry property)
    {
        // Check if the property has the AuditIgnore attribute
        var propertyInfo = property.Metadata.PropertyInfo;
        return propertyInfo?.GetCustomAttributes(typeof(AuditIgnoreAttribute), true).Length != 0 == true;
    }
}
