using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

/// <summary>
/// Default pagination request parameters
/// </summary>
public abstract class PaginatedRequestQueryDTO
{
    /// <summary>
    /// Page number
    /// </summary>
    [Required]
    public virtual int Page { get; init; }

    /// <summary>
    /// Page size
    /// </summary>
    [Required]
    public virtual int PageSize { get; init; }
}
