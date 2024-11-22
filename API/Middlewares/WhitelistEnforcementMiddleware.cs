using API.Authorization;
using API.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace API.Middlewares;

/// <summary>
/// Middleware that enforces all requesting ClaimsPrinciples contain the whitelist claim
/// </summary>
/// <remarks>
/// This middleware should always be registered AFTER the Authentication middleware
/// </remarks>
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

        // If authentication wasn't successful the request will be rejected anyways
        if (context.User.Identity is { IsAuthenticated: false })
        {
            await next(context);
            return;
        }

        // Allow whitelisted and privileged requests
        if (context.User.IsWhitelisted() || context.User.IsAdmin())
        {
            await next(context);
            return;
        }

#if DEBUG
        if (context.Request.Path.ToString().Contains("swagger"))
        {
            await next(context);
            return;
        }
#endif

        string? ident = null;
        try
        {
            ident = context.User.GetSubjectId().ToString();
        }
        catch
        {
            // ignored, we can expect null ids here
        }
        ident ??= "unknown";

        logger.LogInformation(
            "Rejecting {Requester} with identity {Ident} for whitelist violation",
            (context.User.IsClient() ? OtrClaims.Roles.Client : OtrClaims.Roles.User).ToLower(),
            ident
        );

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
    }
}
