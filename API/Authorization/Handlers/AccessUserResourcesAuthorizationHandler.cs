using API.Authorization.Requirements;
using API.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace API.Authorization.Handlers;

/// <summary>
/// Handles authorization for the policy <see cref="AuthorizationPolicies.AccessUserResources"/>
/// </summary>
public class AccessUserResourcesAuthorizationHandler : AuthorizationHandler<AccessUserResourcesRequirement>
{
    /// <inheritdoc/>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AccessUserResourcesRequirement requirement
    )
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var userId = context.User.AuthorizedIdentity();
        // Reject if the request has no user id or does not pass "id" in route
        if (!int.TryParse(httpContext.Request.RouteValues["id"]?.ToString(), out var routeId) || !userId.HasValue)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Allow requests that are privileged or have matching user id and target route id
        if (context.User.IsAdmin() || context.User.IsSystem() || (requirement.AllowSelf && userId == routeId))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
