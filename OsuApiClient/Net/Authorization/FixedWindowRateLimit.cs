namespace OsuApiClient.Net.Authorization;

/// <summary>
/// Represents a fixed window rate limit
/// </summary>
internal class FixedWindowRateLimit
{
    /// <summary>
    /// Timestamp that the window was created
    /// </summary>
    public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Number of tokens remaining in the bucket
    /// </summary>
    public int RemainingTokens { get; set; } = 60;

    /// <summary>
    /// Maximum number of tokens available in the bucket
    /// </summary>
    public int TokenLimit { get; set; } = 60;

    /// <summary>
    /// Timespan that represents the lifetime of the bucket
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Timespan that represents the time the bucket will expire
    /// </summary>
    public TimeSpan ExpiresIn => Window - (DateTimeOffset.Now - Created);

    /// <summary>
    /// Denotes if the bucket has expired
    /// </summary>
    public bool HasExpired => ExpiresIn < TimeSpan.Zero;
}
