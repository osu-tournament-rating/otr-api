using API.Authorization.Requirements;
using API.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace API.Authorization.Handlers;

/// <summary>
/// API key authorization handler. When a requester includes X-Api-Key with a value that
/// matches the configured API key, the request will be authorized.
/// </summary>
/// <remarks>
/// This should only be used in place of [AllowAnonymous] to protect routes from
/// truly anonymous requests.
/// </remarks>
/// <param name="httpContextAccessor">Context accessor</param>
/// <param name="authConfiguration">Auth configuration</param>
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

        // If no key is present, do nothing. The request will likely fail.
        return Task.CompletedTask;
    }
}
