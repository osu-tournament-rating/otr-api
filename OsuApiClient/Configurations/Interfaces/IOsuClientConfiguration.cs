using JetBrains.Annotations;

namespace OsuApiClient.Configurations.Interfaces;

/// <summary>
/// Interfaces configuration values for the osu! API client
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public interface IOsuClientConfiguration
{
    /// <summary>
    /// Gets or sets the client id
    /// </summary>
    long ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret
    /// </summary>
    string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the client redirect url
    /// </summary>
    string RedirectUrl { get; set; }

    /// <summary>
    /// Gets or sets the client rate limit for <see cref="Enums.FetchPlatform.Osu"/>
    /// </summary>
    int OsuRateLimit { get; set; }

    /// <summary>
    /// Gets or sets the client rate limit for <see cref="Enums.FetchPlatform.OsuTrack"/>
    /// </summary>
    int OsuTrackRateLimit { get; set; }

    /// <summary>
    /// Whether to enable distributed locking with redis
    /// </summary>
    bool EnableDistributedLocking { get; set; }
}
