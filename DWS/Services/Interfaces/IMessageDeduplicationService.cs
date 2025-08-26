using OsuApiClient.Enums;

namespace DWS.Services.Interfaces;

/// <summary>
/// Service for preventing duplicate fetch messages across all osu! API resources
/// </summary>
public interface IMessageDeduplicationService
{
    /// <summary>
    /// Attempts to reserve a resource for fetching. Returns true if reserved, false if already pending or recently processed
    /// </summary>
    /// <param name="resourceType">Type of resource to fetch</param>
    /// <param name="resourceId">ID of the resource</param>
    /// <param name="platform">Platform to fetch from</param>
    /// <param name="pendingTtl">Optional TTL for the pending reservation</param>
    /// <returns>True if successfully reserved, false otherwise</returns>
    Task<bool> TryReserveFetchAsync(FetchResourceType resourceType, long resourceId, FetchPlatform platform, TimeSpan? pendingTtl = null);

    /// <summary>
    /// Marks a resource fetch as completed and sets processed flag
    /// </summary>
    /// <param name="resourceType">Type of resource that was fetched</param>
    /// <param name="resourceId">ID of the resource</param>
    /// <param name="platform">Platform that was used</param>
    /// <param name="processedTtl">Optional TTL for the processed cache</param>
    Task MarkFetchCompletedAsync(FetchResourceType resourceType, long resourceId, FetchPlatform platform, TimeSpan? processedTtl = null);

    /// <summary>
    /// Clears all resource fetch reservations
    /// </summary>
    Task ClearAsync();

    /// <summary>
    /// Releases a resource fetch reservation (used on failure or timeout)
    /// </summary>
    /// <param name="resourceType">Type of resource</param>
    /// <param name="resourceId">ID of the resource</param>
    /// <param name="platform">Platform</param>
    Task ReleaseFetchAsync(FetchResourceType resourceType, long resourceId, FetchPlatform platform);

    /// <summary>
    /// Checks if a resource fetch is currently pending or recently processed
    /// </summary>
    /// <param name="resourceType">Type of resource</param>
    /// <param name="resourceId">ID of the resource</param>
    /// <param name="platform">Platform</param>
    /// <returns>Status of the resource fetch</returns>
    Task<DeduplicationStatus> GetFetchStatusAsync(FetchResourceType resourceType, long resourceId, FetchPlatform platform);

    /// <summary>
    /// Batch check for multiple resources of the same type (optimization for tournament processing)
    /// </summary>
    /// <param name="resourceType">Type of resources</param>
    /// <param name="resourceIds">IDs of the resources</param>
    /// <param name="platform">Platform</param>
    /// <returns>Dictionary mapping resource IDs to their deduplication status</returns>
    Task<Dictionary<long, DeduplicationStatus>> GetBatchFetchStatusAsync(FetchResourceType resourceType, IEnumerable<long> resourceIds, FetchPlatform platform);

    /// <summary>
    /// Batch reserve for multiple resources (returns IDs that were successfully reserved)
    /// </summary>
    /// <param name="resourceType">Type of resources</param>
    /// <param name="resourceIds">IDs of the resources to reserve</param>
    /// <param name="platform">Platform</param>
    /// <param name="pendingTtl">Optional TTL for the pending reservations</param>
    /// <returns>List of resource IDs that were successfully reserved</returns>
    Task<List<long>> TryReserveBatchFetchAsync(FetchResourceType resourceType, IEnumerable<long> resourceIds, FetchPlatform platform, TimeSpan? pendingTtl = null);
}
