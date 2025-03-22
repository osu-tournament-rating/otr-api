namespace API.DTOs.Interfaces;

/// <summary>
/// Default pagination request parameters
/// </summary>
public interface IPaginated
{
    /// <summary>
    /// The one-indexed page number
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; init; }
}
