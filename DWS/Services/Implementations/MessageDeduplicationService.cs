using DWS.Configurations;
using DWS.Services.Interfaces;
using Microsoft.Extensions.Options;
using OsuApiClient.Enums;
using StackExchange.Redis;

namespace DWS.Services.Implementations;

/// <summary>
/// Service for preventing duplicate fetch messages across all osu! API resources using Redis
/// </summary>
public class MessageDeduplicationService(
    IConnectionMultiplexer redis,
    IOptions<MessageDeduplicationConfiguration> configuration,
    ILogger<MessageDeduplicationService> logger) : IMessageDeduplicationService
{
    private readonly IDatabase _database = redis.GetDatabase();
    private readonly MessageDeduplicationConfiguration _configuration = configuration.Value;

    /// <summary>
    /// Generates the Redis key for a pending fetch reservation
    /// </summary>
    private static string GetPendingKey(FetchResourceType resourceType, long resourceId, FetchPlatform platform) =>
        $"otr-dws:fetch:{resourceType.ToString().ToLower()}:{platform.ToString().ToLower()}:pending:{resourceId}";

    /// <summary>
    /// Generates the Redis key for a processed fetch cache
    /// </summary>
    private static string GetProcessedKey(FetchResourceType resourceType, long resourceId, FetchPlatform platform) =>
        $"otr-dws:fetch:{resourceType.ToString().ToLower()}:{platform.ToString().ToLower()}:processed:{resourceId}";

    public async Task<bool> TryReserveFetchAsync(
        FetchResourceType resourceType,
        long resourceId,
        FetchPlatform platform,
        TimeSpan? pendingTtl = null)
    {
        if (!_configuration.Enabled || !GetResourceConfiguration(resourceType, platform).Enabled)
        {
            return true; // Deduplication disabled, allow all fetches
        }

        string pendingKey = GetPendingKey(resourceType, resourceId, platform);
        string processedKey = GetProcessedKey(resourceType, resourceId, platform);

        // Check if already processed recently
        if (await _database.KeyExistsAsync(processedKey))
        {
            logger.LogDebug("{ResourceType} {ResourceId} recently processed for {Platform}",
                resourceType, resourceId, platform);
            return false;
        }

        // Try to set pending flag (returns true if key was set, false if it already existed)
        TimeSpan ttl = pendingTtl ?? GetConfiguredPendingTtl(resourceType, platform);
        bool reserved = await _database.StringSetAsync(
            pendingKey,
            DateTime.UtcNow.ToString("O"),
            ttl,
            When.NotExists);

        if (!reserved)
        {
            logger.LogDebug("{ResourceType} {ResourceId} already pending for {Platform}",
                resourceType, resourceId, platform);
        }
        else
        {
            logger.LogDebug("Reserved {ResourceType} {ResourceId} for {Platform} fetch with TTL {Ttl}",
                resourceType, resourceId, platform, ttl);
        }

        return reserved;
    }

    public async Task MarkFetchCompletedAsync(
        FetchResourceType resourceType,
        long resourceId,
        FetchPlatform platform,
        TimeSpan? processedTtl = null)
    {
        if (!_configuration.Enabled || !GetResourceConfiguration(resourceType, platform).Enabled)
        {
            return; // Deduplication disabled
        }

        string pendingKey = GetPendingKey(resourceType, resourceId, platform);
        string processedKey = GetProcessedKey(resourceType, resourceId, platform);

        // Remove pending flag
        await _database.KeyDeleteAsync(pendingKey);

        // Set processed flag with TTL
        TimeSpan ttl = processedTtl ?? GetConfiguredProcessedTtl(resourceType, platform);
        await _database.StringSetAsync(
            processedKey,
            DateTime.UtcNow.ToString("O"),
            ttl);

        logger.LogDebug("Marked {ResourceType} {ResourceId} as processed for {Platform} with TTL {Ttl}",
            resourceType, resourceId, platform, ttl);
    }

    public async Task ReleaseFetchAsync(
        FetchResourceType resourceType,
        long resourceId,
        FetchPlatform platform)
    {
        if (!_configuration.Enabled || !GetResourceConfiguration(resourceType, platform).Enabled)
        {
            return; // Deduplication disabled
        }

        string pendingKey = GetPendingKey(resourceType, resourceId, platform);
        bool deleted = await _database.KeyDeleteAsync(pendingKey);

        if (deleted)
        {
            logger.LogDebug("Released reservation for {ResourceType} {ResourceId} on {Platform}",
                resourceType, resourceId, platform);
        }
    }

    public async Task<DeduplicationStatus> GetFetchStatusAsync(
        FetchResourceType resourceType,
        long resourceId,
        FetchPlatform platform)
    {
        if (!_configuration.Enabled || !GetResourceConfiguration(resourceType, platform).Enabled)
        {
            return DeduplicationStatus.Available; // Deduplication disabled
        }

        string pendingKey = GetPendingKey(resourceType, resourceId, platform);
        string processedKey = GetProcessedKey(resourceType, resourceId, platform);

        if (await _database.KeyExistsAsync(pendingKey))
        {
            return DeduplicationStatus.Pending;
        }

        if (await _database.KeyExistsAsync(processedKey))
        {
            return DeduplicationStatus.RecentlyProcessed;
        }

        return DeduplicationStatus.Available;
    }

    public async Task<Dictionary<long, DeduplicationStatus>> GetBatchFetchStatusAsync(
        FetchResourceType resourceType,
        IEnumerable<long> resourceIds,
        FetchPlatform platform)
    {
        // Materialize the enumerable once to avoid multiple enumeration
        var resourceIdList = resourceIds.ToList();
        var result = new Dictionary<long, DeduplicationStatus>();

        if (!_configuration.Enabled || !GetResourceConfiguration(resourceType, platform).Enabled)
        {
            // Deduplication disabled, all resources are available
            foreach (long resourceId in resourceIdList)
            {
                result[resourceId] = DeduplicationStatus.Available;
            }
            return result;
        }

        // Use Redis batch/pipeline for efficiency
        IBatch? batch = _database.CreateBatch();
        var pendingTasks = new Dictionary<long, Task<bool>>();
        var processedTasks = new Dictionary<long, Task<bool>>();

        foreach (long resourceId in resourceIdList)
        {
            string pendingKey = GetPendingKey(resourceType, resourceId, platform);
            string processedKey = GetProcessedKey(resourceType, resourceId, platform);

            pendingTasks[resourceId] = batch.KeyExistsAsync(pendingKey);
            processedTasks[resourceId] = batch.KeyExistsAsync(processedKey);
        }

        batch.Execute();
        await Task.WhenAll(pendingTasks.Values.Concat(processedTasks.Values));

        foreach (long resourceId in resourceIdList)
        {
            if (await pendingTasks[resourceId])
            {
                result[resourceId] = DeduplicationStatus.Pending;
            }
            else if (await processedTasks[resourceId])
            {
                result[resourceId] = DeduplicationStatus.RecentlyProcessed;
            }
            else
            {
                result[resourceId] = DeduplicationStatus.Available;
            }
        }

        logger.LogDebug("Batch status check for {Count} {ResourceType} resources: {Available} available, {Pending} pending, {Processed} processed",
            resourceIdList.Count,
            resourceType,
            result.Count(r => r.Value == DeduplicationStatus.Available),
            result.Count(r => r.Value == DeduplicationStatus.Pending),
            result.Count(r => r.Value == DeduplicationStatus.RecentlyProcessed));

        return result;
    }

    public async Task<List<long>> TryReserveBatchFetchAsync(
        FetchResourceType resourceType,
        IEnumerable<long> resourceIds,
        FetchPlatform platform,
        TimeSpan? pendingTtl = null)
    {
        // Materialize the enumerable once to avoid multiple enumeration
        var resourceIdList = resourceIds.ToList();
        var reserved = new List<long>();

        if (!_configuration.Enabled || !GetResourceConfiguration(resourceType, platform).Enabled)
        {
            // Deduplication disabled, reserve all
            return resourceIdList;
        }

        // First, check status of all resources
        Dictionary<long, DeduplicationStatus> statuses = await GetBatchFetchStatusAsync(resourceType, resourceIdList, platform);

        // Filter to only available resources
        var availableIds = statuses.Where(s => s.Value == DeduplicationStatus.Available).Select(s => s.Key).ToList();

        if (availableIds.Count == 0)
        {
            logger.LogInformation("All {Count} {ResourceType} resources are already pending or recently processed for {Platform}",
                resourceIdList.Count, resourceType, platform);
            return reserved;
        }

        // Try to reserve available resources
        TimeSpan ttl = pendingTtl ?? GetConfiguredPendingTtl(resourceType, platform);
        IBatch? batch = _database.CreateBatch();
        var reservationTasks = new Dictionary<long, Task<bool>>();

        foreach (long resourceId in availableIds)
        {
            string pendingKey = GetPendingKey(resourceType, resourceId, platform);
            reservationTasks[resourceId] = batch.StringSetAsync(
                pendingKey,
                DateTime.UtcNow.ToString("O"),
                ttl,
                When.NotExists);
        }

        batch.Execute();
        await Task.WhenAll(reservationTasks.Values);

        // Collect successfully reserved IDs
        foreach (KeyValuePair<long, Task<bool>> kvp in reservationTasks)
        {
            if (await kvp.Value)
            {
                reserved.Add(kvp.Key);
            }
        }

        logger.LogInformation("Batch reserved {Reserved}/{Available}/{Total} {ResourceType} resources for {Platform}",
            reserved.Count, availableIds.Count, resourceIdList.Count, resourceType, platform);

        return reserved;
    }

    /// <summary>
    /// Gets the configuration for a specific resource type and platform
    /// </summary>
    private FetchDeduplicationSettings GetResourceConfiguration(FetchResourceType resourceType, FetchPlatform platform) =>
        (resourceType, platform) switch
        {
            (FetchResourceType.Player, FetchPlatform.Osu) => _configuration.PlayerFetch,
            (FetchResourceType.Player, FetchPlatform.OsuTrack) => _configuration.PlayerOsuTrackFetch,
            (FetchResourceType.Beatmap, _) => _configuration.BeatmapFetch,
            (FetchResourceType.Match, _) => _configuration.MatchFetch,
            _ => new FetchDeduplicationSettings() // Default settings
        };

    /// <summary>
    /// Gets the configured pending TTL for a resource type and platform
    /// </summary>
    private TimeSpan GetConfiguredPendingTtl(FetchResourceType resourceType, FetchPlatform platform)
    {
        FetchDeduplicationSettings settings = GetResourceConfiguration(resourceType, platform);
        return TimeSpan.FromSeconds(settings.PendingTtlSeconds);
    }

    /// <summary>
    /// Gets the configured processed TTL for a resource type and platform
    /// </summary>
    private TimeSpan GetConfiguredProcessedTtl(FetchResourceType resourceType, FetchPlatform platform)
    {
        FetchDeduplicationSettings settings = GetResourceConfiguration(resourceType, platform);
        return TimeSpan.FromSeconds(settings.ProcessedTtlSeconds);
    }
}
