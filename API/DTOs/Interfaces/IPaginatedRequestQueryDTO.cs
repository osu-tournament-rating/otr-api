namespace API.DTOs.Interfaces;

/// <summary>
/// Default pagination request parameters
/// </summary>
public interface IPaginatedRequestQueryDTO
{
    /// <summary>
    /// Page number
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; init; }
}
