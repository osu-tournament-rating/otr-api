using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerGen;

/// <summary>
/// Helper methods for various Swagger and OpenApi tasks
/// </summary>
public static class SwaggerGenExtensions
{
    /// <summary>
    /// Adds an <see cref="Microsoft.OpenApi.Interfaces.IOpenApiExtension"/> to an <see cref="OpenApiOperation"/>
    /// that denotes whether the operation requires authorization or not
    /// </summary>
    /// <param name="value">Whether the operation requires authorization or not</param>
    public static void AddAuthExtension(this OpenApiOperation operation, bool value) =>
        operation.Extensions.Add(ExtensionKeys.OperationRequiresAuthorization, new OpenApiBoolean(value));

    /// <summary>
    /// Gets all <see cref="Attribute"/>s of a given type from the action and its controller
    /// </summary>
    /// <typeparam name="TAttr">The type of <see cref="Attribute"/> to filter for</typeparam>
    public static IEnumerable<TAttr> GetControllerAndActionAttributes<TAttr>(this OperationFilterContext context)
        where TAttr : Attribute
    {
        if (context.MethodInfo is null)
        {
            return [];
        }

        return context.MethodInfo
            .GetCustomAttributes<TAttr>()
            .Concat(context.MethodInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes<TAttr>() ?? []);
    }

    /// <summary>
    /// Creates an <see cref="OpenApiArray"/> of <see cref="OpenApiString"/> from a list of strings
    /// </summary>
    public static OpenApiArray ToOpenApiArray(this IEnumerable<string> list)
    {
        var output = new OpenApiArray();
        output.AddRange(list.Select(str => new OpenApiString(str)));
        return output;
    }
}
