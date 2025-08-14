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
/// Background service that periodically checks for players with outdated osu!track data and enqueues messages to fetch updated data from the osu!track API.
/// </summary>
public class PlayerOsuTrackUpdateBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<PlayerOsuTrackUpdateBackgroundService> logger,
    IOptions<PlayerOsuTrackUpdateServiceConfiguration> configuration) : BackgroundService
{
    private readonly PlayerOsuTrackUpdateServiceConfiguration _configuration = configuration.Value;

    /// <summary>
    /// Executes the background service.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token to stop the service.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_configuration.Enabled)
        {
            logger.LogInformation("Player osu!track update background service is disabled via configuration");
            return;
        }

        logger.LogInformation("Player osu!track update background service started [Check Interval: {CheckInterval}s | Outdated After: {OutdatedAfterDays} days | Max Messages Per Cycle: {MaxMessages} | Priority: {Priority}]",
            _configuration.CheckIntervalSeconds, _configuration.OutdatedAfterDays, _configuration.MaxMessagesPerCycle, (MessagePriority)_configuration.MessagePriority);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndEnqueueOutdatedPlayersAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogDebug("Player osu!track update check cancelled");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while checking for outdated osu!track player data");
            }

            await Task.Delay(TimeSpan.FromSeconds(_configuration.CheckIntervalSeconds), stoppingToken);
        }
    }

    /// <summary>
    /// Checks for players with outdated osu!track data and enqueues messages to fetch updated data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task CheckAndEnqueueOutdatedPlayersAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        OtrContext context = scope.ServiceProvider.GetRequiredService<OtrContext>();
        IPublishEndpoint publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        IMessageDeduplicationService deduplicationService = scope.ServiceProvider.GetRequiredService<IMessageDeduplicationService>();

        DateTime cutoffDate = DateTime.UtcNow.AddDays(-_configuration.OutdatedAfterDays);

        logger.LogDebug("Checking for players with osu!track data older than {CutoffDate:u} [Max Messages: {MaxMessages}]",
            cutoffDate, _configuration.MaxMessagesPerCycle);

        var outdatedPlayers = await context.Players
            .AsNoTracking()
            .Where(p => p.OsuTrackLastFetch < cutoffDate)
            .OrderBy(p => p.OsuTrackLastFetch) // Process oldest first
            .Take(_configuration.MaxMessagesPerCycle)
            .Select(p => new { p.Id, p.OsuId, p.Username, p.OsuTrackLastFetch })
            .ToListAsync(cancellationToken);

        if (outdatedPlayers.Count == 0)
        {
            logger.LogDebug("No outdated osu!track player data found");
            return;
        }

        logger.LogInformation("Found {Count} players with outdated osu!track data", outdatedPlayers.Count);

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
                    FetchPlatform.OsuTrack))
                {
                    logger.LogDebug("Skipping player {OsuId} ({Username}) - already pending or recently processed for osu!track",
                        player.OsuId, player.Username);
                    skippedCount++;
                    continue;
                }

                try
                {
                    var message = new FetchPlayerOsuTrackMessage
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
                    logger.LogInformation("Enqueued player osu!track fetch message [Username: {Username} | Last Fetch: {LastFetch:u} | Days Outdated: {DaysOutdated:F1}]",
                        player.Username, player.OsuTrackLastFetch, (DateTime.UtcNow - player.OsuTrackLastFetch).TotalDays);
                }
                catch (Exception ex)
                {
                    await deduplicationService.ReleaseFetchAsync(
                        FetchResourceType.Player,
                        player.OsuId,
                        FetchPlatform.OsuTrack);

                    failedCount++;
                    logger.LogError(ex, "Failed to enqueue osu!track message for player [Username: {Username}]", player.Username);
                }
            }
        }

        if (failedCount > 0 || skippedCount > 0)
        {
            logger.LogWarning("Player osu!track update cycle completed [Enqueued: {EnqueuedCount} | Skipped: {SkippedCount} | Failed: {FailedCount} | Total: {TotalCount}]",
                enqueuedCount, skippedCount, failedCount, outdatedPlayers.Count);
        }
        else if (enqueuedCount > 0)
        {
            logger.LogInformation("Successfully enqueued {EnqueuedCount} player osu!track fetch messages", enqueuedCount);
        }
    }

    /// <summary>
    /// Triggered when the service is stopping.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Player osu!track update background service is stopping");
        await base.StopAsync(cancellationToken);
    }
}
