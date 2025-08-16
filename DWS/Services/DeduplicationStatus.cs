namespace DWS.Services;

/// <summary>
/// Status of a resource fetch deduplication check
/// </summary>
public enum DeduplicationStatus
{
    /// <summary>
    /// Resource is available for fetching
    /// </summary>
    Available,

    /// <summary>
    /// Resource fetch is currently pending
    /// </summary>
    Pending,

    /// <summary>
    /// Resource was recently processed and is cached
    /// </summary>
    RecentlyProcessed
}
