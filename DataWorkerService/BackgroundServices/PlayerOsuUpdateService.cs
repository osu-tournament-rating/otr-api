using DataWorkerService.Configurations;
using DataWorkerService.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DataWorkerService.BackgroundServices;

/// <summary>
/// Background service tasked with updating outdated osu! API data for <see cref="Database.Entities.Player"/>s
/// </summary>
public class PlayerOsuUpdateService(
    ILogger<PlayerOsuUpdateService> logger,
    IServiceProvider serviceProvider,
    IOptions<PlayerFetchConfiguration> playerDataWorkerConfig
) : ScopeConsumingBackgroundService(logger, serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly PlayerFetchPlatformConfiguration _config = playerDataWorkerConfig.Value.Osu;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_config.Enabled)
        {
            logger.LogInformation("Background service disabled due to configuration");
            return;
        }

        if (_config.MarkAllOutdated)
        {
            logger.LogInformation("Marking all Players' osu! API data as outdated due to configuration");
            await MarkAllOutdated();
        }

        await base.ExecuteAsync(stoppingToken);
    }

    protected override async Task OnWorkCompleted(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(_config.BatchIntervalSeconds), stoppingToken);
    }

    protected override async Task DoWork(IServiceScope scope, CancellationToken stoppingToken)
    {
        IPlayersService playersService = scope.ServiceProvider.GetRequiredService<IPlayersService>();

        await playersService.UpdateOutdatedFromOsuApiAsync(_config);
    }

    private async Task MarkAllOutdated()
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IPlayersService playersService = scope.ServiceProvider.GetRequiredService<IPlayersService>();

        await playersService.SetAllOutdatedOsuApiAsync(_config);
    }
}
