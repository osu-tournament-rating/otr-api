using API.Utilities;
using API.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace API.Middlewares;

/// <summary>
/// Middleware that enforces all requesting ClaimsPrinciples contain the whitelist claim
/// </summary>
public class WhitelistEnforcementMiddleware(RequestDelegate next, ILogger<WhitelistEnforcementMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        // Check for [AllowAnonymous]
        if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            await next(context);
            return;
        }

        // Allow whitelisted and privileged requests
        if (context.User.IsWhitelisted() || context.User.IsAdmin() || context.User.IsSystem())
        {
            await next(context);
            return;
        }

        logger.LogInformation("Rejecting client with identity {id} for whitelist violation", context.User.AuthorizedIdentity());
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
    }
}
