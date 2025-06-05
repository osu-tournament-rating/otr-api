namespace API.Configurations;

public class AuthConfiguration
{
    public const string Position = "Auth";

    /// <summary>
    /// Allows CORS requests from the configured origins
    /// </summary>
    public string[] AllowedHosts { get; init; } = [];

    /// <summary>
    /// If the global whitelist should be enforced
    /// </summary>
    public bool EnforceWhitelist { get; init; }

    /// <summary>
    /// If Data Protection keys should be persisted to Redis
    /// </summary>
    public bool PersistDataProtectionKeys { get; init; } = true;

    /// <summary>
    /// The expiration time for cookies
    /// </summary>
    public TimeSpan CookieExpiration { get; init; } = TimeSpan.FromDays(30);
}
