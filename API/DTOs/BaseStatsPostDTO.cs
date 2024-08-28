using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Used for POSTing base statistics to the API
/// </summary>
public class BaseStatsPostDTO
{
    public int PlayerId { get; set; }
    public double MatchCostAverage { get; set; }
    public double Rating { get; set; }
    public double Volatility { get; set; }
    public Ruleset Ruleset { get; set; }
    public double Percentile { get; set; }
    public int GlobalRank { get; set; }
    public int CountryRank { get; set; }
}
