using API.Utilities;

namespace API.DTOs;

/// <summary>
/// General stats that are current and not time specific, i.e. current rating, volatility, tier
/// </summary>
public class BaseStatsDTO
{
    public int PlayerId { get; set; }
    public double Rating { get; set; }
    public double Volatility { get; set; }
    public int Mode { get; set; }
    public double Percentile { get; set; }
    public int MatchesPlayed { get; set; }
    public double Winrate { get; set; }
    public int HighestGlobalRank { get; set; }
    public int GlobalRank { get; set; }
    public int CountryRank { get; set; }
    public double AverageMatchCost { get; set; }
    public int TournamentsPlayed { get; set; }
    public RankProgressDTO RankProgress { get; set; } = new();
    public bool IsProvisional => RatingUtils.IsProvisional(Volatility, MatchesPlayed, TournamentsPlayed);
}
