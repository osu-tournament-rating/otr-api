using DataWorkerService.Configurations;
using DataWorkerService.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DataWorkerService.BackgroundServices;

/// <summary>
/// Background service tasked with updating outdated osu!Track API data for <see cref="Database.Entities.Player"/>s
/// </summary>
public class PlayerOsuTrackUpdateService(
    ILogger<PlayerOsuTrackUpdateService> logger,
    IServiceProvider serviceProvider,
    IOptions<PlayerFetchConfiguration> playerDataWorkerConfig
) : ScopeConsumingBackgroundService(logger, serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly PlayerFetchPlatformConfiguration _config = playerDataWorkerConfig.Value.OsuTrack;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_config.Enabled)
        {
            logger.LogInformation("Background service disabled due to configuration");
            return;
        }

        if (_config.MarkAllOutdated)
        {
            logger.LogInformation("Marking all Players' osu!Track API data as outdated due to configuration");
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

        await playersService.UpdateOutdatedFromOsuTrackApiAsync(_config);
    }

    private async Task MarkAllOutdated()
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IPlayersService playersService = scope.ServiceProvider.GetRequiredService<IPlayersService>();

        await playersService.SetAllOutdatedOsuTrackApiAsync(_config);
    }
}
