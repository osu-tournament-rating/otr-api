using API.Authorization.Requirements;
using API.Configurations;
using API.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace API.Authorization.Handlers;

/// <summary>
/// Determines whether authorization is allowed for a <see cref="WhitelistAuthorizationRequirement"/>
/// </summary>
public class WhitelistAuthorizationHandler(
    IOptions<AuthConfiguration> authConfiguration
) : AuthorizationHandler<WhitelistAuthorizationRequirement>
{
    private readonly bool _enabled = authConfiguration.Value.EnforceWhitelist;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        WhitelistAuthorizationRequirement requirement
    )
    {
        if (!_enabled || (_enabled && context.User.IsWhitelisted()))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}
