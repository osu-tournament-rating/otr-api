using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace API.HealthChecks;

/// <summary>
/// Health check for data protection services
/// </summary>
public class DataProtectionHealthCheck(IDataProtectionProvider dataProtectionProvider) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            const string testData = "test";

            IDataProtector protector = dataProtectionProvider.CreateProtector("health-check-test");
            string protectedData = protector.Protect(testData);
            string unprotectedData = protector.Unprotect(protectedData);

            bool isHealthy = testData == unprotectedData;

            return Task.FromResult(isHealthy
                ? HealthCheckResult.Healthy("Data Protection keys accessible")
                : HealthCheckResult.Unhealthy("Data Protection key validation failed"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Data Protection health check failed", ex));
        }
    }
}
