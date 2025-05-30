using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerGen.Filters;

/// <summary>
/// Marks all schema properties as required if they are not nullable
/// </summary>
[UsedImplicitly]
public class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        IEnumerable<string> requiredProps = schema.Properties
            .Where(prop => !prop.Value.Nullable && !schema.Required.Contains(prop.Key))
            .Select(prop => prop.Key);

        foreach (string prop in requiredProps)
        {
            schema.Required.Add(prop);
        }
    }
}
