using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Common.Enums;
using Database.Entities.Interfaces;
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
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; }

    public DateTime Created { get; } = DateTime.UtcNow;

    public int ReferenceIdLock { get; private set; }

    public int? ReferenceId { get; private set; }

    public int? ActionUserId { get; set; }

    public AuditActionType ActionType { get; private set; }

    public string? Before { get; private set; }
    public string? After { get; private set; }

    public virtual bool GenerateAudit(EntityEntry origEntityEntry, IHttpContextAccessor? httpContextAccessor)
    {
        if (origEntityEntry.Entity is not IEntity originalEntity)
        {
            return false;
        }

        // Determine action type
        AuditActionType? auditAction = origEntityEntry.State switch
        {
            EntityState.Added => AuditActionType.Created,
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
        // Else: ActionUserId remains null, indicating a system action or anonymous user.

        JsonSerializerOptions serializerOptions = new()
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };

        if (ActionType == AuditActionType.Created)
        {
            After = JsonSerializer.Serialize(origEntityEntry.CurrentValues.ToObject(), serializerOptions);
        }
        else if (ActionType == AuditActionType.Deleted)
        {
            Before = JsonSerializer.Serialize(origEntityEntry.OriginalValues.ToObject(), serializerOptions);
        }
        else if (ActionType == AuditActionType.Updated)
        {
            // Only audit if properties have actually changed.
            // This check is important because EF Core might mark an entity as Modified
            // even if no property values have changed (e.g. if a related entity was modified).
            bool changed = origEntityEntry.Properties.Any(p => p.IsModified && !Equals(p.CurrentValue, p.OriginalValue));
            if (!changed)
            {
                // Also check collection modifications
                foreach (var collectionEntry in origEntityEntry.Collections)
                {
                    if (collectionEntry.IsModified)
                    {
                        changed = true;
                        break;
                    }
                }
            }

            if (!changed)
            {
                // And reference modifications
                foreach (var referenceEntry in origEntityEntry.References)
                {
                    if (referenceEntry.IsModified)
                    {
                        changed = true;
                        break;
                    }
                }
            }


            if (!changed)
            {
                return false; // No actual changes to log
            }

            Before = JsonSerializer.Serialize(origEntityEntry.OriginalValues.ToObject(), serializerOptions);
            After = JsonSerializer.Serialize(origEntityEntry.CurrentValues.ToObject(), serializerOptions);
        }

        return true;
    }
}
