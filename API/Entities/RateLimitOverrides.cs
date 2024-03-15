namespace API.Entities;

public class RateLimitOverrides
{
    public RateLimitOverrides() { }
    public RateLimitOverrides(int? permitLimit, int? window)
    {
        PermitLimit = permitLimit;
        Window = window;
    }
    public int? PermitLimit { get; set; }
    public int? Window { get; set; }
}
