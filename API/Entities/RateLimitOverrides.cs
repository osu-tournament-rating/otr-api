namespace API.Entities;

public class RateLimitOverrides
{
    public RateLimitOverrides() { }
    public RateLimitOverrides(int? tokenLimit, int? tokensPerPeriod, int? replenishmentPeriod)
    {
        TokenLimit = tokenLimit;
        TokensPerPeriod = tokensPerPeriod;
        ReplenishmentPeriod = replenishmentPeriod;
    }
    public int? TokenLimit { get; set; }
    public int? TokensPerPeriod { get; set; }
    public int? ReplenishmentPeriod { get; set; }
}
