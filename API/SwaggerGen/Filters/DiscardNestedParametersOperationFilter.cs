using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerGen.Filters;

/// <summary>
/// Removes unnecessary nested parameters from operations
/// </summary>
[UsedImplicitly]
public class DiscardNestedParametersOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var nestedParams = operation.Parameters
            .Where(p => p.Name.Contains('.'))
            .ToList();

        foreach (OpenApiParameter param in nestedParams)
        {
            operation.Parameters.Remove(param);
        }
    }
}
