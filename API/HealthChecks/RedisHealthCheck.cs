using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace API.HealthChecks;

public class RedisHealthCheck(IConnectionMultiplexer redis) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (redis == null)
            {
                return HealthCheckResult.Degraded("No Redis connection configured or resolved.");
            }

            var database = redis.GetDatabase();
            TimeSpan pong = await database.PingAsync();

            return HealthCheckResult.Healthy($"Redis connection healthy - {pong.TotalMilliseconds}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis connection failed", ex);
        }
    }
}
