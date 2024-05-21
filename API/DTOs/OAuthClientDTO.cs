using API.Entities;

namespace API.DTOs;

/// <summary>
/// Represents an OAuth client
/// </summary>
public class OAuthClientDTO
{
    /// <summary>
    /// Client id of the client
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    /// List of permissions granted to the client
    /// </summary>
    public string[] Scopes { get; set; } = [];

    /// <summary>
    /// Possible rate limit overrides for the client
    /// </summary>
    public RateLimitOverrides RateLimitOverrides { get; set; } = new();
}
