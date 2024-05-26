namespace OsuApiClient.Net.Authorization;

/// <summary>
/// Represents a set of bearer token credentials
/// </summary>
public class OsuCredentials
{
    /// <summary>
    /// Timestamp the credentials were created
    /// </summary>
    public readonly DateTimeOffset Created = DateTimeOffset.Now;

    /// <summary>
    /// Access token
    /// </summary>
    public string AccessToken { get; internal set; } = null!;

    /// <summary>
    /// Refresh token
    /// </summary>
    public string? RefreshToken { get; internal set; }

    /// <summary>
    /// Represents the lifetime of the access token in seconds
    /// </summary>
    public long ExpiresInSeconds { get; internal set; }

    /// <summary>
    /// Timespan that represents the time the access token will expire
    /// </summary>
    public TimeSpan ExpiresIn => TimeSpan.FromSeconds(ExpiresInSeconds) - (DateTimeOffset.Now - Created);

    /// <summary>
    /// Denotes if the access token has expired
    /// </summary>
    public bool HasExpired => ExpiresIn < TimeSpan.Zero;
}
