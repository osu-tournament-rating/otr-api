namespace API.Repositories.Interfaces;

/// <summary>
/// Interface for repositories that have interactions with the redis cache
/// </summary>
public interface IUsesCache
{
    /// <summary>
    /// Invalidates related cache entities
    /// </summary>
    Task InvalidateCacheEntriesAsync();
}
