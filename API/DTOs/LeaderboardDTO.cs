namespace API.DTOs;

public class LeaderboardDTO
{
    public int Mode { get; set; }
    public int TotalPlayerCount { get; set; }
    public LeaderboardFilterDefaultsDTO FilterDefaults { get; set; } = new();
    public IEnumerable<LeaderboardPlayerInfoDTO> Leaderboard { get; set; } =
        new List<LeaderboardPlayerInfoDTO>();

    /// <summary>
    ///  Data displayed if the user is logged in
    /// </summary>
    public LeaderboardPlayerChartDTO? PlayerChart { get; set; }
}
