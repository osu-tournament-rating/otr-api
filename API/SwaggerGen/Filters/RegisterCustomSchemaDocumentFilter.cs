using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerGen.Filters;

/// <summary>
/// Registers a custom <see cref="OpenApiSchema"/> to an <see cref="OpenApiDocument"/>
/// </summary>
/// <param name="name">The name for the given schema</param>
/// <param name="schema">The <see cref="OpenApiSchema"/> to register</param>
[UsedImplicitly]
public class RegisterCustomSchemaDocumentFilter(string name, OpenApiSchema schema) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        context.SchemaRepository.AddDefinition(name, schema);
    }
}
