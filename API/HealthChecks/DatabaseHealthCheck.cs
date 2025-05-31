using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace API.HealthChecks;

/// <summary>
/// Performs a health check on the database
/// </summary>
/// <param name="context">The database context</param>
public class DatabaseHealthCheck(OtrContext context) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext healthCheckContext,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (context == null)
            {
                return HealthCheckResult.Unhealthy("Database context not available (not resolved from DI).");
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            bool canConnect = await context.Database.CanConnectAsync(cancellationToken);

            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy("Cannot connect to database.");
            }

            await context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);

            stopwatch.Stop();

            return HealthCheckResult.Healthy($"Database connection healthy - {stopwatch.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database health check failed", ex);
        }
    }
}
