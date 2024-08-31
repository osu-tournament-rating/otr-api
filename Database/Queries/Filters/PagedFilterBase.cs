using System.ComponentModel.DataAnnotations;

namespace Database.Queries.Filters;

/// <summary>
/// Query filter base for endpoints supporting pagination
/// </summary>
public class PagedFilterBase
{
    /// <summary>
    /// Controls the number of results to include per page
    /// </summary>
    [Range(1, int.MaxValue)]
    public virtual int Limit { get; set; } = 100;

    /// <summary>
    /// Controls the desired page of results
    /// </summary>
    [Range(1, int.MaxValue)]
    public virtual int Page { get; set; } = 1;
}
