using Microsoft.Extensions.Options;
using OsuApiClient.Configurations.Interfaces;

namespace OsuApiClient;

/// <summary>
/// Represents options that control behaviors in the osu! API client environment
/// </summary>
public class OsuClientOptions : IOptions<OsuClientOptions>
{
    /// <summary>
    /// Gets or sets the client configuration
    /// </summary>
    public IOsuClientConfiguration? Configuration { get; init; }

    /// <summary>
    /// Gets or sets whether or not to register services for the environment as scoped
    /// </summary>
    /// <remarks>True by default</remarks>
    public bool UseScopedServices { get; init; } = true;

    /// <summary>
    /// Gets the instance of the current options
    /// </summary>
    public OsuClientOptions Value => this;
}
