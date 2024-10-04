using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerGen;

/// <summary>
/// Generates <see cref="OpenApiSecurityRequirement"/>(s) for each route based on any <see cref="AuthorizeAttribute"/>(s)
/// </summary>
public class ActionSecurityOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (GetControllerAndActionAttributes<AllowAnonymousAttribute>(context).Any())
        {
            return;
        }

        IEnumerable<AuthorizeAttribute> authAttributes = GetControllerAndActionAttributes<AuthorizeAttribute>(context).ToList();
        if (!authAttributes.Any())
        {
            return;
        }

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
            var claims = authAttributes
                .Where(attr => attr.Roles != null)
                .Select(attr => attr.Roles!.Split(", "))
                .SelectMany(c => c)
                .Distinct()
                .ToList();

            operation.Security.Add(SecurityRequirements.OAuthSecurityRequirementFromClaims(claims));

            desc += "\n\nClaim(s): " + string.Join(", ", claims);
        }

        operation.Description += desc;
    }

    private static IEnumerable<TAttr> GetControllerAndActionAttributes<TAttr>(OperationFilterContext context)
        where TAttr : Attribute
    {
        if (context.MethodInfo is null)
        {
            return new List<TAttr>();
        }

        return context.MethodInfo.GetCustomAttributes<TAttr>()
            .Concat(
                context.MethodInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes<TAttr>() ?? Array.Empty<TAttr>()
            );
    }
}
