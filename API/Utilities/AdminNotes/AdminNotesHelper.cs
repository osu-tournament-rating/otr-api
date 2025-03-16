using System.Reflection;
using Database;
using Database.Entities.Interfaces;

namespace API.Utilities.AdminNotes;

public static class AdminNotesHelper
{
    /// <summary>
    /// Gets all types that implement <see cref="IAdminNotableEntity{TAdminNote}"/>
    /// </summary>
    public static IEnumerable<Type> GetAdminNoteableEntityTypes() =>
        Assembly.GetAssembly(typeof(OtrContext))
            ?.GetTypes()
            .Where(t =>
                t is { IsClass: true, IsAbstract: false }
                && t.GetInterfaces().Any(
                    i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAdminNotableEntity<>)
                )
            ) ?? [];

    /// <summary>
    /// Gets all valid API route segments for admin note actions
    /// </summary>
    public static IEnumerable<string> GetAdminNoteableEntityRoutes() =>
        GetAdminNoteableEntityTypes().Select(t => t.Name.ToLower());

    /// <summary>
    /// Gets the type for the concrete implementation of an <see cref="IAdminNoteEntity"/> for a given type that
    /// implements <see cref="IAdminNotableEntity{TAdminNote}"/>
    /// </summary>
    /// <returns>The type of <see cref="IAdminNoteEntity"/> if available</returns>
    public static Type? GetAdminNoteType(this Type type) =>
     type.GetInterfaces()
         .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAdminNotableEntity<>))
         ?.GetGenericArguments()
         .FirstOrDefault();
}
