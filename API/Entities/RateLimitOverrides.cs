using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

/// <summary>
/// Represents values used to override the default rate limit configuration
/// </summary>
public class RateLimitOverrides
{
    /// <summary>
    /// The number of requests granted per window
    /// </summary>
    [Column("permit_limit")]
    public int? PermitLimit { get; set; }

    /// <summary>
    /// The length of the window in seconds
    /// </summary>
    [Column("window")]
    public int? Window { get; set; }
}
