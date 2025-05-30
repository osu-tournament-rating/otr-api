using API.Utilities.Extensions;
using Database.Entities.Interfaces;
using Database.Interceptors;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace API.Utilities;

/// <summary>
/// Extends the <see cref="AuditingInterceptor"/> to automatically blame actions on the currently authenticated user
/// </summary>
/// <param name="httpContextAccessor">Http context accessor</param>
[UsedImplicitly]
public class AuditBlamingInterceptor(IHttpContextAccessor httpContextAccessor) : AuditingInterceptor
{
    private readonly HttpContext? _httpContext = httpContextAccessor.HttpContext;

    protected override void OnSavingChanges(DbContext context)
    {
        // Generate audits
        base.OnSavingChanges(context);

        // Get the currently authenticated user's id from the http context
        if (_httpContext == null || _httpContext.User.Identity is { IsAuthenticated: false })
        {
            return;
        }

        if (!_httpContext.User.TryGetSubjectId(out int? subjectId))
        {
            return;
        }

        // Blame any created audits
        context.ChangeTracker
            .Entries<IAuditEntity>()
            .ToList()
            .ForEach(entry => entry.Entity.ActionUserId = subjectId);
    }
}
