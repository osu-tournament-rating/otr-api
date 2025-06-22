using System.ComponentModel.DataAnnotations;

namespace Database.Entities;

/// <summary>
/// Stores a filtering request and its result for transparency
/// </summary>
public class FilterReport : EntityBase
{
    /// <summary>
    /// The ID of the user who made the filtering request
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// The filtering request data serialized as JSON
    /// </summary>
    [Required]
    public string RequestJson { get; set; } = string.Empty;

    /// <summary>
    /// The filtering response data serialized as JSON
    /// </summary>
    [Required]
    public string ResponseJson { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the user who made the request
    /// </summary>
    public virtual User User { get; set; } = null!;
}
