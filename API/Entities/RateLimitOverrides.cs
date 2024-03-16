using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

/// <summary>
/// Denotes values used to override the rate limit configuration
/// </summary>
public class RateLimitOverrides
{
    [Column("permit_limit")]
    public int? PermitLimit { get; set; }

    [Column("window")]
    public int? Window { get; set; }
}
