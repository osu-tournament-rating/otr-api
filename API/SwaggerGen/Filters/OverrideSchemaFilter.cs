using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerGen.Filters;

/// <summary>
/// Simple schema filter that executes the given action when the filter
/// encounters the given type
/// </summary>
[UsedImplicitly]
public class OverrideSchemaFilter<T>(Action<OpenApiSchema, SchemaFilterContext> filter) : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(T))
        {
            filter(schema, context);
        }
    }
}
