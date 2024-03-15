using System.ComponentModel.DataAnnotations;

namespace API.Configurations;

public class RateLimitConfiguration
{
    public const string Position = "RateLimit";

    /// <summary>
    /// The total amount of tokens available per <see cref="Window"/>
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "PermitLimit must be an integer greater than 1!")]
    public int PermitLimit { get; } = 30;
    /// <summary>
    /// The amount of time (in seconds) before the <see cref="PermitLimit"/> is refreshed
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Window must be an integer greater than 1!")]
    public int Window { get; } = 30;
}
