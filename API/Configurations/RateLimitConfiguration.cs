namespace API.Configurations;

public class RateLimitConfiguration
{
    public const string Position = "RateLimit";

    /// <summary>
    /// The total amount of tokens available per bucket
    /// </summary>
    public int TokenLimit { get; } = 30;
    /// <summary>
    /// The amount of tokens returned to the bucket per <see cref="RateLimitConfiguration.ReplenishmentPeriod"/>
    /// </summary>
    public int TokensPerPeriod { get; } = 30;
    /// <summary>
    /// The amount of time (in seconds) between token replenishment
    /// </summary>
    public int ReplenishmentPeriod { get; } = 60;
}
