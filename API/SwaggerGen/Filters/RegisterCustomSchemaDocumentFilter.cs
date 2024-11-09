using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerGen.Filters;

/// <summary>
/// Registers a custom <see cref="OpenApiSchema"/> to an <see cref="OpenApiDocument"/>
/// </summary>
/// <param name="type">The type described by the schema</param>
/// <param name="schema">A schema for the given type</param>
[UsedImplicitly]
public class RegisterCustomSchemaDocumentFilter(Type type, OpenApiSchema schema) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        context.SchemaRepository.AddDefinition(type.Name, schema);
    }
}
