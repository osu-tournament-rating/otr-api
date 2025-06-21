using System.Diagnostics;

namespace DataWorkerService.BackgroundServices;

/// <summary>
/// Base implementation of a <see cref="BackgroundService"/>
/// that consumes a <see cref="IServiceScope"/> to complete a unit of work
/// </summary>
/// <param name="logger">Logger</param>
public abstract class ScopeConsumingBackgroundService(
    ILogger logger,
    IServiceProvider serviceProvider
) : BackgroundService
{
    private readonly Stopwatch _stopwatch = new();

    private int _executionCount;

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Background service stopping [Runs: {RunsCount}]", _executionCount);

        await base.StopAsync(stoppingToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await OnExecutingAsync(stoppingToken);
    }

    protected virtual async Task OnExecutingAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Background service initialized");

        while (!stoppingToken.IsCancellationRequested)
        {
            _executionCount++;
            logger.LogTrace("Background service beginning work [Run: {CurRun}]", _executionCount);

            _stopwatch.Start();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                await DoWork(scope, stoppingToken);
            }

            _stopwatch.Stop();
            logger.LogTrace(
                @"Background service completed work [Run: {CurRun} | Elapsed: {Elapsed:mm\:ss\:fff}]",
                _executionCount,
                _stopwatch.Elapsed
            );
            _stopwatch.Reset();

            await OnWorkCompleted(stoppingToken);
        }
    }

    /// <summary>
    /// Called during <see cref="OnExecutingAsync"/> after the <see cref="IServiceScope"/> has been consumed
    /// </summary>
    /// <remarks>Can be used to set a delay between executions of <see cref="DoWork"/></remarks>
    /// <param name="stoppingToken">Stopping token</param>
    protected virtual Task OnWorkCompleted(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called during <see cref="OnExecutingAsync"/> when the <see cref="IServiceScope"/> is created
    /// to perform the unit of work
    /// </summary>
    /// <remarks>The scope is immediately disposed upon completion</remarks>
    /// <param name="scope">Service scope</param>
    /// <param name="stoppingToken">Stopping token</param>
    protected abstract Task DoWork(IServiceScope scope, CancellationToken stoppingToken);
}
