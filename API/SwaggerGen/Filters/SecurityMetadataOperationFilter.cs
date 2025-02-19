using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerGen.Filters;

/// <summary>
/// Enriches the metadata of an <see cref="OpenApiOperation"/> based on any <see cref="AuthorizeAttribute"/>(s).
/// Adds an extension to each action that denotes if the action requires authorization or not.
/// If the action requires authorization, adds additional documentation to the description of an action based on
/// authorization requirement(s).
/// </summary>
[UsedImplicitly]
public class SecurityMetadataOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.GetControllerAndActionAttributes<AllowAnonymousAttribute>().Any())
        {
            operation.AddAuthExtension(false);
            return;
        }

        IEnumerable<AuthorizeAttribute> authAttributes = [.. context.GetControllerAndActionAttributes<AuthorizeAttribute>()];
        if (!authAttributes.Any())
        {
            operation.AddAuthExtension(false);
            return;
        }

        operation.AddAuthExtension(true);
        operation.Security.Add(SecurityRequirements.BearerSecurityRequirement);

        var desc = "\n\nRequires Authorization:";

        if (authAttributes.Any(attr => attr.Policy != null))
        {
            IEnumerable<string?> policies = authAttributes
                .Where(attr => attr.Policy != null)
                .Select(attr => attr.Policy);

            desc += "\n\nPolicy: " + string.Join(", ", policies);
        }

        if (authAttributes.Any(attr => attr.Roles != null))
        {
            var roles = authAttributes
                .Where(attr => attr.Roles != null)
                .Select(attr => attr.Roles!.Split(", "))
                .SelectMany(c => c)
                .Distinct()
                .ToList();

            operation.Security.Add(SecurityRequirements.OAuthSecurityRequirement(roles));

            desc += "\n\nClaim(s): " + string.Join(", ", roles);
        }

        operation.Description += desc;
    }
}
