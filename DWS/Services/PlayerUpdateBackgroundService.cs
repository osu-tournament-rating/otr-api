using Common.Enums;
using Database;
using DWS.Configurations;
using DWS.Messages;
using DWS.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OsuApiClient.Enums;

namespace DWS.Services;

/// <summary>
/// Background service that periodically checks for players with outdated data and enqueues messages to fetch updated data from the osu! API.
/// </summary>
public class PlayerUpdateBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<PlayerUpdateBackgroundService> logger,
    IOptions<PlayerUpdateServiceConfiguration> configuration) : BackgroundService
{
    private readonly PlayerUpdateServiceConfiguration _configuration = configuration.Value;

    /// <summary>
    /// Executes the background service.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token to stop the service.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_configuration.Enabled)
        {
            logger.LogInformation("Player update background service is disabled via configuration");
            return;
        }

        logger.LogInformation("Player update background service started [Check Interval: {CheckInterval}s | Outdated After: {OutdatedAfterDays} days | Max Messages Per Cycle: {MaxMessages} | Priority: {Priority}]",
            _configuration.CheckIntervalSeconds, _configuration.OutdatedAfterDays, _configuration.MaxMessagesPerCycle, (MessagePriority)_configuration.MessagePriority);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndEnqueueOutdatedPlayersAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogDebug("Player update check cancelled");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while checking for outdated players");
            }

            await Task.Delay(TimeSpan.FromSeconds(_configuration.CheckIntervalSeconds), stoppingToken);
        }
    }

    /// <summary>
    /// Checks for players with outdated data and enqueues messages to fetch updated data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task CheckAndEnqueueOutdatedPlayersAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        OtrContext context = scope.ServiceProvider.GetRequiredService<OtrContext>();
        IPublishEndpoint publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        IMessageDeduplicationService deduplicationService = scope.ServiceProvider.GetRequiredService<IMessageDeduplicationService>();

        DateTime cutoffDate = DateTime.UtcNow.AddDays(-_configuration.OutdatedAfterDays);

        logger.LogDebug("Checking for players with data older than {CutoffDate:u} [Max Messages: {MaxMessages}]",
            cutoffDate, _configuration.MaxMessagesPerCycle);

        var outdatedPlayers = await context.Players
            .AsNoTracking()
            .Where(p => p.OsuLastFetch < cutoffDate)
            .OrderBy(p => p.OsuLastFetch) // Process oldest first
            .Take(_configuration.MaxMessagesPerCycle)
            .Select(p => new { p.Id, p.OsuId, p.Username, p.OsuLastFetch })
            .ToListAsync(cancellationToken);

        if (outdatedPlayers.Count == 0)
        {
            logger.LogDebug("No outdated players found");
            return;
        }

        logger.LogInformation("Found {Count} players with outdated data", outdatedPlayers.Count);

        int enqueuedCount = 0;
        int skippedCount = 0;
        int failedCount = 0;

        foreach (var player in outdatedPlayers)
        {
            var correlationId = Guid.NewGuid();

            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["PlayerId"] = player.Id,
                ["OsuPlayerId"] = player.OsuId,
                ["CorrelationId"] = correlationId
            }))
            {
                if (!await deduplicationService.TryReserveFetchAsync(
                    FetchResourceType.Player,
                    player.OsuId,
                    FetchPlatform.Osu))
                {
                    logger.LogDebug("Skipping player {OsuId} ({Username}) - already pending or recently processed",
                        player.OsuId, player.Username);
                    skippedCount++;
                    continue;
                }

                try
                {
                    var message = new FetchPlayerMessage
                    {
                        OsuPlayerId = player.OsuId,
                        CorrelationId = correlationId,
                        Priority = (MessagePriority)_configuration.MessagePriority
                    };

                    await publishEndpoint.Publish(message, publishContext =>
                    {
                        publishContext.SetPriority(_configuration.MessagePriority);
                        publishContext.CorrelationId = message.CorrelationId;
                    }, cancellationToken);

                    enqueuedCount++;
                    logger.LogInformation("Enqueued player fetch message [Username: {Username} | Last Fetch: {LastFetch:u} | Days Outdated: {DaysOutdated:F1}]",
                        player.Username, player.OsuLastFetch, (DateTime.UtcNow - player.OsuLastFetch).TotalDays);
                }
                catch (Exception ex)
                {
                    await deduplicationService.ReleaseFetchAsync(
                        FetchResourceType.Player,
                        player.OsuId,
                        FetchPlatform.Osu);

                    failedCount++;
                    logger.LogError(ex, "Failed to enqueue message for player [Username: {Username}]", player.Username);
                }
            }
        }

        if (failedCount > 0 || skippedCount > 0)
        {
            logger.LogWarning("Player update cycle completed [Enqueued: {EnqueuedCount} | Skipped: {SkippedCount} | Failed: {FailedCount} | Total: {TotalCount}]",
                enqueuedCount, skippedCount, failedCount, outdatedPlayers.Count);
        }
        else if (enqueuedCount > 0)
        {
            logger.LogInformation("Successfully enqueued {EnqueuedCount} player fetch messages", enqueuedCount);
        }
    }

    /// <summary>
    /// Triggered when the service is stopping.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Player update background service is stopping");
        await base.StopAsync(cancellationToken);
    }
}
