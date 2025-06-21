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
    /// Gets the instance of the current options
    /// </summary>
    public OsuClientOptions Value => this;
}
