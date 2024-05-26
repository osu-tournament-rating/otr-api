using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OsuApiClient.Net.Authorization;

namespace OsuApiClient.Tests;

public class ClientTestService(ILogger<ClientTestService> logger, IOsuClient osuClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = await osuClient.UpdateCredentialsAsync(stoppingToken);
    }
}
