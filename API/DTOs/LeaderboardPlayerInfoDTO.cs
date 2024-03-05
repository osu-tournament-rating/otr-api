namespace API.DTOs;

/// <summary>
///  Individual line items in the leaderboard
/// </summary>
public class LeaderboardPlayerInfoDTO
{
    public int PlayerId { get; set; }
    public long OsuId { get; set; }
    public int GlobalRank { get; set; }
    public string Name { get; set; } = null!;
    public string Tier { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int MatchesPlayed { get; set; }
    public double WinRate { get; set; }
    public int Mode { get; set; }
    public string? Country { get; set; }
}
