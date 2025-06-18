using API.DTOs;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerGen.Filters;

/// <summary>
/// Custom schema filter to handle MatchDTO inheritance properly for TypeScript generation.
/// This resolves issues with NSwag's TypeScript generator when dealing with property shadowing
/// using the 'new' keyword in derived classes.
/// </summary>
[UsedImplicitly]
public class MatchDtoSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(MatchDTO))
        {
            return;
        }

        // Don't process if this is a reference to MatchDTO, only process the actual schema definition
        if (schema.Reference != null)
        {
            return;
        }

        // Check if we're dealing with an inheritance scenario (allOf with base class)
        if (!(schema.AllOf?.Count > 0) || schema.Properties?.Count != 0)
        {
            return;
        }

        // Find all schemas that contain properties
        var schemasWithProperties = schema.AllOf
            .Where(s => s.Properties?.Count > 0)
            .ToList();

        // Merge all properties from all schemas
        var allProperties = new Dictionary<string, OpenApiSchema>();
        var allRequired = new HashSet<string>();

        foreach (OpenApiSchema? subSchema in schemasWithProperties)
        {
            // Copy properties, with later schemas overriding earlier ones (handles shadowing)
            foreach (KeyValuePair<string, OpenApiSchema> prop in subSchema.Properties)
            {
                allProperties[prop.Key] = prop.Value;
            }

            // Merge required properties
            if (!(subSchema.Required?.Count > 0))
            {
                continue;
            }

            foreach (string? req in subSchema.Required)
            {
                allRequired.Add(req);
            }
        }

        // Apply merged properties to the main schema
        schema.Properties = allProperties;

        // Apply merged required properties
        if (allRequired.Count > 0)
        {
            schema.Required = allRequired;
        }

        // Clear the allOf to remove inheritance representation
        schema.AllOf.Clear();

        // Ensure the description is preserved
        schema.Description ??= "Represents a played match";
    }
}
