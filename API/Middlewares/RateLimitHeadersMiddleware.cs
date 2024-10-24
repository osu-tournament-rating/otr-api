using System.Threading.RateLimiting;
using API.Configurations;
using API.Utilities.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace API.Middlewares;

/// <summary>
/// Middleware that adds rate limit information to the headers of outbound responses
/// </summary>
public class RateLimitHeadersMiddleware(
    RequestDelegate next,
    IOptions<RateLimiterOptions> rateLimiterOptions,
    IOptions<RateLimitConfiguration> rateLimiterConfiguration
)
{
    private readonly RateLimitConfiguration _rateLimitConfiguration = rateLimiterConfiguration.Value;
    private readonly PartitionedRateLimiter<HttpContext> _rateLimiter = rateLimiterOptions.Value.GlobalLimiter!;

    public async Task Invoke(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: false })
        {
            await next(context);
            return;
        }

        RateLimiterStatistics? statistics = _rateLimiter.GetStatistics(context);
        if (statistics is null)
        {
            await next(context);
            return;
        }

        context.Response.Headers.Append(
            "X-RateLimit-Limit",
            (context.User.GetRateLimitOverrides()?.PermitLimit ?? _rateLimitConfiguration.PermitLimit).ToString()
        );
        context.Response.Headers.Append("X-RateLimit-Remaining", statistics.CurrentAvailablePermits.ToString());

        await next(context);
    }
}
