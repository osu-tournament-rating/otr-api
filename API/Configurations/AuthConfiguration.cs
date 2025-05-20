using JetBrains.Annotations;

namespace API.Configurations;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
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
}
