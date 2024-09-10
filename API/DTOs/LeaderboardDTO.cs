using Database.Enums;

namespace API.DTOs;

public class LeaderboardDTO
{
    public Ruleset Ruleset { get; set; }
    public int TotalPlayerCount { get; set; }
    public LeaderboardFilterDefaultsDTO FilterDefaults { get; set; } = new();
    public IEnumerable<LeaderboardPlayerInfoDTO> Leaderboard { get; set; } =
        new List<LeaderboardPlayerInfoDTO>();
}
