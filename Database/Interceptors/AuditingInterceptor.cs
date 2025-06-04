using Database.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Database.Interceptors;

/// <summary>
/// An implementation of <see cref="ISaveChangesInterceptor"/> that audits changes to database entities
/// </summary>
public class AuditingInterceptor(IHttpContextAccessor? httpContextAccessor) : ISaveChangesInterceptor
{
    private readonly IHttpContextAccessor? _httpContextAccessor = httpContextAccessor;

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

    protected virtual void OnSavingChanges(DbContext context)
    {
        // Cache the current change list to avoid detecting changes multiple times
        var trackedEntries = context.ChangeTracker.Entries().ToList();

        // Get all entities that have a corresponding audit entity (by convention: [EntityName]Audit)
        // and are in a state that should be audited.
        IEnumerable<EntityEntry> auditableEntries = trackedEntries.Where(entry =>
            entry.State is EntityState.Modified or EntityState.Deleted &&
            GetAuditType(entry.Entity.GetType()) != null
        );

        // Create audits
        foreach (EntityEntry entry in auditableEntries)
        {
            CreateAudit(entry, context, _httpContextAccessor);
        }
    }

    private static void CreateAudit(EntityEntry entry, DbContext context, IHttpContextAccessor? httpContextAccessor)
    {
        Type entityType = entry.Entity.GetType();
        Type? auditType = GetAuditType(entityType);

        if (auditType is null)
        {
            // This should not happen if auditableEntries is filtered correctly
            return;
        }

        var newAudit = (IAuditEntity?)Activator.CreateInstance(auditType);
        if (newAudit is null)
        {
            return;
        }

        // Populate the audit's properties and attach it to the context for creation
        bool success = newAudit.GenerateAudit(entry, httpContextAccessor);
        if (success)
        {
            context.Attach(newAudit);
        }
    }

    private static Type? GetAuditType(Type entityType)
    {
        string auditEntityTypeName = $"{entityType.FullName}Audit";
        return entityType.Assembly.GetType(auditEntityTypeName);
    }
}
