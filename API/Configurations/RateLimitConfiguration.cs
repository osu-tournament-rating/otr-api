using System.ComponentModel.DataAnnotations;

namespace API.Configurations;

/// <summary>
/// Configures the default rate limit for all authorized users and clients
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class RateLimitConfiguration
{
    public const string Position = "RateLimit";

    /// <summary>
    /// The total amount of tokens available per <see cref="Window"/>
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "PermitLimit must be an integer greater than 1!")]
    public int PermitLimit { get; init; } = 60;

    /// <summary>
    /// The amount of time (in seconds) before the <see cref="PermitLimit"/> is refreshed
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Window must be an integer greater than 1!")]
    public int Window { get; init; } = 60;
}
