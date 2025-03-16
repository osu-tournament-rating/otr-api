using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerGen.Filters;

public class OverrideSchemaFilter<T>(Action<OpenApiSchema, SchemaFilterContext> filter) : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Type != null && schema.Type.GetType() == typeof(T))
        {
            filter(schema, context);
        }
    }
}
