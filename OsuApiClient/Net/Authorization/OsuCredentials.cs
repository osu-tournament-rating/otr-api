namespace OsuApiClient.Net.Authorization;

/// <summary>
/// Represents a set of bearer token credentials
/// </summary>
public class OsuCredentials
{
    /// <summary>
    /// Timestamp the credentials were created
    /// </summary>
    private readonly DateTimeOffset _created = DateTimeOffset.Now;

    /// <summary>
    /// Access token
    /// </summary>
    public string AccessToken { get; init; } = null!;

    /// <summary>
    /// Refresh token
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Represents the lifetime of the access token in seconds
    /// </summary>
    public long ExpiresInSeconds { get; init; }

    /// <summary>
    /// Timespan that represents the time the access token will expire
    /// </summary>
    public TimeSpan ExpiresIn => TimeSpan.FromSeconds(ExpiresInSeconds) - (DateTimeOffset.Now - _created);

    /// <summary>
    /// Denotes if the access token has expired or is about to expire
    /// </summary>
    /// <remarks>
    /// Includes a 5-minute buffer to prevent using tokens that are about to expire
    /// </remarks>
    public bool HasExpired => ExpiresIn < TimeSpan.FromMinutes(5);
}
