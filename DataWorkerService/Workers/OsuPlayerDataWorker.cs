using DataWorkerService.Configurations;
using DataWorkerService.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DataWorkerService.Workers;

/// <summary>
/// Background service tasked with updating outdated <see cref="Database.Entities.Player"/> osu! API data
/// </summary>
public class OsuPlayerDataWorker(
    ILogger<OsuPlayerDataWorker> logger,
    IOptions<PlayerDataWorkerConfiguration> playerDataWorkerConfig,
    IServiceProvider serviceProvider
) : BackgroundService
{
    private int _executionCount;

    private readonly PlayerPlatformConfiguration _config = playerDataWorkerConfig.Value.Osu;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_config.AllowFetch)
        {
            logger.LogInformation(
                "Skipping initialization of data worker due to configuration [Worker: {WorkerName}]",
                nameof(OsuPlayerDataWorker)
            );
            return;
        }

        if (_config.MarkAllOutdated)
        {
            logger.LogInformation(
                "Marking all Players as having outdated osu! API data due to configuration [Worker: {WorkerName}]",
                nameof(OsuPlayerDataWorker)
            );
            await MarkAllOutdated();
        }

        await BackgroundTask(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Stopping data worker [Worker: {WorkerName} | Runs: {RunsCount}]",
            nameof(OsuPlayerDataWorker),
            _executionCount
        );

        await base.StopAsync(stoppingToken);
    }

    private async Task BackgroundTask(CancellationToken stoppingToken)
    {
        logger.LogInformation("Initialized data worker [Worker: {WorkerName}]", nameof(OsuPlayerDataWorker));

        while (!stoppingToken.IsCancellationRequested)
        {
            _executionCount++;

            logger.LogDebug(
                "Data worker beginning work [Worker: {WorkerName} | Run: {CurRun}]",
                nameof(OsuPlayerDataWorker),
                _executionCount
            );

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                IPlayersService playersService = scope.ServiceProvider.GetRequiredService<IPlayersService>();

                await playersService.UpdateOutdatedFromOsuApiAsync(_config);
            }

            logger.LogDebug(
                "Data worker completed work [Worker: {WorkerName} | Run: {CurRun}]",
                nameof(OsuPlayerDataWorker),
                _executionCount
            );

            await Task.Delay(TimeSpan.FromSeconds(_config.BatchIntervalSeconds), stoppingToken);
        }
    }

    private async Task MarkAllOutdated()
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IPlayersService playersService = scope.ServiceProvider.GetRequiredService<IPlayersService>();

        await playersService.SetAllOutdatedOsuApiAsync(_config);
    }
}
