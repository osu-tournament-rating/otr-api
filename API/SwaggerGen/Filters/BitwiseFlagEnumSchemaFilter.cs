using System.Reflection;
using JetBrains.Annotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerGen.Filters;

/// <summary>
/// Appends a custom attribute to the schemas generated for enums that are bitwise flags
/// </summary>
[UsedImplicitly]
public class BitwiseFlagEnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        Type? type = context.Type;
        if (type is not null && type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() is not null)
        {
            schema.Extensions.Add("x-bitwiseFlag", new OpenApiBoolean(true));
        }
    }
}
