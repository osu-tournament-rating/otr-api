using System.Diagnostics.CodeAnalysis;
using Database.Entities.Interfaces;
using JetBrains.Annotations;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Utilities.AdminNotes;

/// <summary>
/// Type of entity to target for admin note actions
/// </summary>
public class AdminNoteRouteTarget : IParsable<AdminNoteRouteTarget>
{
    /// <summary>
    /// Original input to the route segment
    /// </summary>
    [SwaggerIgnore]
    public string Original { get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// Type of the parent entity
    /// </summary>
    [SwaggerIgnore]
    [UsedImplicitly]
    public Type EntityType { get; init; } = null!;

    /// <summary>
    /// Type of the admin note entity
    /// </summary>
    [SwaggerIgnore]
    public Type AdminNoteType { get; [UsedImplicitly] init; } = null!;

    public static AdminNoteRouteTarget Parse(string s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out AdminNoteRouteTarget? result))
        {
            throw new ArgumentException("Could not parse admin note route target", nameof(s));
        }

        return result;
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out AdminNoteRouteTarget result
    )
    {
        result = null;

        if (string.IsNullOrEmpty(s))
        {
            return false;
        }

        Type? entityType = AdminNotesHelper
            .GetAdminNoteableEntityTypes()
            .FirstOrDefault(t => t.ToAdminNoteableEntityRoute() == s);

        Type? adminNoteType = entityType?.GetAdminNoteType();
        if (entityType is null || adminNoteType is null || !typeof(IAdminNoteEntity).IsAssignableFrom(adminNoteType))
        {
            return false;
        }

        result = new AdminNoteRouteTarget { Original = s, EntityType = entityType, AdminNoteType = adminNoteType };

        return true;
    }
}
