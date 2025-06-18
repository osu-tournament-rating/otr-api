using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a paged list of results
/// </summary>
/// <typeparam name="T">Type of DTO that is being returned</typeparam>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class PagedResultDTO<T> where T : class
{
    /// <summary>
    /// Link to the next potential page of results
    /// </summary>
    public string? Next { get; set; }

    /// <summary>
    /// Link to the previous potential page of results
    /// </summary>
    public string? Previous { get; set; }

    /// <summary>
    /// Number of results included
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// List of resulting data
    /// </summary>
    public required ICollection<T> Results { get; set; }
}
