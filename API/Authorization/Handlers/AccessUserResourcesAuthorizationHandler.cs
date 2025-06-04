using API.Authorization.Requirements;
using API.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace API.Authorization.Handlers;

/// <summary>
/// Determines whether authorization is allowed for a <see cref="AccessUserResourcesAuthorizationRequirement"/>
/// </summary>
public class AccessUserResourcesAuthorizationHandler(
    IHttpContextAccessor httpContextAccessor
) : AuthorizationHandler<AccessUserResourcesAuthorizationRequirement>
{
    private readonly HttpContext? _httpContext = httpContextAccessor.HttpContext;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AccessUserResourcesAuthorizationRequirement requirement
    )
    {
        if (_httpContext is null || context.User.Identity is not { IsAuthenticated: true })
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Reject if the request does not pass "id" in route
        if (int.TryParse(_httpContext.Request.RouteValues["id"]?.ToString(), out int routeId))
        {
            // Allow requests that are privileged or have matching user id and target route id
            if (context.User.IsAdmin() || context.User.GetSubjectId() == routeId)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        context.Fail();
        return Task.CompletedTask;
    }
}
