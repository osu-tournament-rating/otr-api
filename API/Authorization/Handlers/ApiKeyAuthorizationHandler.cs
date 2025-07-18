using API.Authorization.Requirements;
using API.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace API.Authorization.Handlers;

public class ApiKeyAuthorizationHandler(
    IHttpContextAccessor httpContextAccessor,
    IOptions<AuthConfiguration> authConfiguration) :
    AuthorizationHandler<ApiKeyAuthorizationRequirement>
{
    private const string Header = "X-Api-Key";

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ApiKeyAuthorizationRequirement requirement)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null || !httpContext.Request.Headers.TryGetValue(Header, out StringValues providedKey))
        {
            return Task.CompletedTask;
        }

        string expectedKey = authConfiguration.Value.AuthorizationApiKey;

        if (expectedKey.Equals(providedKey))
        {
            context.Succeed(requirement);
        }

        // If no key is present, do nothing. Another authorization requirement could succeed.
        return Task.CompletedTask;
    }
}
