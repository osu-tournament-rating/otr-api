using OsuApiClient.Enums;

namespace OsuApiClient.Net.Authorization;

/// <summary>
/// Represents a fixed window rate limit
/// </summary>
internal sealed class FixedWindowRateLimit(FetchPlatform platform, int? rateLimitOverride)
{
    /// <summary>
    /// Timestamp that the window was created
    /// </summary>
    private DateTimeOffset Created { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Timespan that represents the lifetime of the bucket
    /// </summary>
    private TimeSpan Window { get; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Maximum number of tokens available in the bucket
    /// </summary>
    public int TokenLimit { get; } = rateLimitOverride ?? 60;

    /// <summary>
    /// Number of tokens remaining in the bucket
    /// </summary>
    public int RemainingTokens { get; private set; } = rateLimitOverride ?? 60;

    /// <summary>
    /// Timespan that represents the time the bucket will expire
    /// </summary>
    public TimeSpan ExpiresIn => Window - (DateTimeOffset.Now - Created);

    /// <summary>
    /// Denotes if the bucket has expired
    /// </summary>
    public bool HasExpired => ExpiresIn < TimeSpan.Zero;

    /// <summary>
    /// Resets the rate limit
    /// </summary>
    public void Reset()
    {
        Created = DateTimeOffset.Now;
        RemainingTokens = TokenLimit;
    }

    /// <summary>
    /// Subtracts 1 from <see cref="RemainingTokens"/>
    /// </summary>
    public void DecrementRemainingTokens()
    {
        RemainingTokens -= 1;
    }
}
