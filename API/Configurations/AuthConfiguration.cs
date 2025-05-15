namespace API.Configurations;

public class AuthConfiguration
{
    public const string Position = "Auth";

    /// <summary>
    /// If the global whitelist should be enforced
    /// </summary>
    public bool EnforceWhitelist { get; init; }

    /// <summary>
    /// Allows CORS requests from the configured origins
    /// </summary>
    public string AllowedHosts { get; init; } = string.Empty;
}
