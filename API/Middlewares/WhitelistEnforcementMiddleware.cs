using API.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace API.Middlewares;

/// <summary>
/// Middleware that enforces all requesting ClaimsPrinciples contain the whitelist claim
/// </summary>
public class WhitelistEnforcementMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        // Check for [AllowAnonymous]
        if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            await next(context);
            return;
        }

        if (!context.User.IsWhitelisted() || !context.User.IsAdmin() || !context.User.IsSystem())
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else
        {
            await next(context);
        }
    }
}
