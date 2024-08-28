using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OsuApiClient.Tests.Tests;

namespace OsuApiClient.Tests;

/// <summary>
/// Background service that runs integrated tests against an <see cref="OsuClient"/>
/// </summary>
public class ClientTestService(
    ILogger<ClientTestService> logger,
    IServiceProvider serviceProvider,
    IHostApplicationLifetime lifetime
    ) : BackgroundService
{
    private readonly IEnumerable<IOsuClientTest> _clientTests = serviceProvider.GetServices<IOsuClientTest>();

    /// <summary>
    /// Runs all registered implementations of <see cref="IOsuClientTest"/> against the registered
    /// implementation of the <see cref="IOsuClient"/>
    /// </summary>
    /// <param name="stoppingToken">Cancellation token</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Running {Count} test(s)", _clientTests.Count());

        var passedCount = 0;
        foreach (IOsuClientTest test in _clientTests)
        {
            try
            {
                var passed = await test.RunAsync(stoppingToken);

                if (passed)
                {
                    logger.LogInformation("✔️ {Name}", test.Name);
                    passedCount++;
                }
                else
                {
                    logger.LogError("❌ {Name}", test.Name);
                }
            }
            catch (Exception e)
            {
                logger.LogError("❌ {Name}", test.Name);
                logger.LogError("{Exception}", e.Message);
            }
        }

        logger.LogInformation("{Passed} / {Total} test(s) passed", passedCount, _clientTests.Count());
        lifetime.StopApplication();
    }
}
