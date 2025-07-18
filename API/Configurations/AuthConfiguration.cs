using System.ComponentModel.DataAnnotations;

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
    /// The key used to determine whether requests are coming from
    /// a resource within the o!TR domain, such as otr-web.
    /// </summary>
    /// <remarks>
    /// Requests with a header of X-Api-Key: {AuthorizationApiKey}
    /// and no other metadata will have requests approved of,
    /// assuming the endpoint has this authorization policy.
    /// This key is a way for us to approve public web requests
    /// without enabling [AllowAnonymous] on public endpoints.
    /// This way, we can still rely on user-created API clients
    /// for access to public resources.
    /// </remarks>
    [Required(ErrorMessage = $"{nameof(AuthorizationApiKey)} is required!")]
    [MinLength(8, ErrorMessage = $"{nameof(AuthorizationApiKey)} must be >= 8 characters!")]
    public string AuthorizationApiKey { get; init; } = string.Empty;

    /// <summary>
    /// The expiration time for cookies
    /// </summary>
    public TimeSpan CookieExpiration { get; init; } = TimeSpan.FromDays(30);
}
