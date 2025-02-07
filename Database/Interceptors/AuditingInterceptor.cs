using Database.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Database.Interceptors;

/// <summary>
/// An implementation of <see cref="ISaveChangesInterceptor"/> that audits changes to database entities
/// </summary>
public class AuditingInterceptor : ISaveChangesInterceptor
{
    private readonly List<EntityEntry> _newEntries = [];

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

    public async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is null || _newEntries.Count <= 0)
        {
            return result;
        }

        foreach (EntityEntry entry in _newEntries)
        {
            CreateAudit(entry, eventData.Context);
        }
        _newEntries.Clear();

        await eventData.Context.SaveChangesAsync(cancellationToken);

        return result;
    }

    public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (eventData.Context is null || _newEntries.Count <= 0)
        {
            return result;
        }

        foreach (EntityEntry entry in _newEntries)
        {
            CreateAudit(entry, eventData.Context);
        }
        _newEntries.Clear();

        eventData.Context.SaveChanges();

        return result;
    }

    protected virtual void OnSavingChanges(DbContext context)
    {
        // Cache the current change list to avoid detecting changes multiple times
        var trackedEntries = context.ChangeTracker.Entries().ToList();

        // Get all entities in the change tracker that implement IAuditableEntity<>
        IEnumerable<EntityEntry> auditableEntries = trackedEntries
            .Where(entry =>
                entry.Entity.GetType().GetInterfaces().Any(i =>
                    i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IAuditableEntity<>)
                )
                && entry.State is EntityState.Modified or EntityState.Added or EntityState.Deleted
            )
            .ToList();

        // Create audits
        foreach (EntityEntry entry in auditableEntries)
        {
            // Newly created entities should be processed after changes are saved
            // so that primary keys are created before the audit is created
            if (entry.State is EntityState.Added)
            {
                _newEntries.Add(entry);
                continue;
            }

            CreateAudit(entry, context);
        }
    }

    private static void CreateAudit(EntityEntry entry, DbContext context)
    {
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
            return;
        }

        var newAudit = (IAuditEntity?)Activator.CreateInstance(auditType);
        if (newAudit is null)
        {
            return;
        }

        // Populate the audit's properties and attach it to the context for creation
        var success = newAudit.GenerateAudit(entry);
        if (success)
        {
            context.Attach(newAudit);
        }
    }
}
