using Database.Entities.Interfaces;
using Database.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Database.Interceptors;

/// <summary>
/// An implementation of <see cref="ISaveChangesInterceptor"/> that creates audits of entities that support it
/// </summary>
public class AuditingInterceptor : ISaveChangesInterceptor
{
    public ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is not null)
        {
            OnSavingChanges(eventData.Context);
        }

        return ValueTask.FromResult(result);
    }

    public InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (eventData.Context is not null)
        {
            OnSavingChanges(eventData.Context);
        }

        return result;
    }

    private static void OnSavingChanges(DbContext context)
    {
        // Get all entities in the change tracker that implement IAuditableEntity<>
        IEnumerable<EntityEntry> auditableEntries = context.ChangeTracker.Entries()
            .Where(entry =>
                entry.Entity.GetType().GetInterfaces().Any(i =>
                    i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IAuditableEntity<>)
                )
                && entry.State is EntityState.Modified
            )
            .ToList();

        // Create audits
        foreach (EntityEntry entry in auditableEntries)
        {
            AuditActionType? auditAction = entry.State switch
            {
                EntityState.Modified => AuditActionType.Updated,
                _ => null
            };

            if (!auditAction.HasValue)
            {
                continue;
            }

            // Determine type of the audit entity
            Type? auditType = entry.Entity
                .GetType()
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAuditableEntity<>))
                 ?.GetGenericArguments()
                .FirstOrDefault();

            // Being careful with type safety since we do some casting
            if (auditType is null || !typeof(IAuditEntity).IsAssignableFrom(auditType))
            {
                continue;
            }

            if (entry.Entity is not IEntity entity)
            {
                continue;
            }

            // Find an existing audit ("blamed" on a user, handled by the repo)
            EntityEntry? auditEntry = context.ChangeTracker.Entries()
                .FirstOrDefault(e =>
                    e.Entity.GetType() == auditType
                    && ((IAuditEntity)e.Entity).ReferenceId == entity.Id
                );

            if (auditEntry is null)
            {
                // Create a new audit (non-"blamed")
                var newAudit = (IAuditEntity?)Activator.CreateInstance(auditType);
                if (newAudit is null)
                {
                    continue;
                }

                newAudit.ReferenceId = entity.Id;
                // Attach the new entry to the context so it is created when changes are saved
                auditEntry = context.Attach(newAudit);
            }

            // Populate the audit's properties
            auditEntry.Property(nameof(IAuditEntity.ActionType)).CurrentValue = auditAction;
            ((IAuditEntity)auditEntry.Entity).GenerateAudit(entry, auditEntry);
        }
    }
}
