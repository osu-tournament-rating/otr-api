using Common.Enums.Enums;

namespace API.DTOs;

public class LeaderboardDTO
{
    public Ruleset Ruleset { get; set; }
    public IEnumerable<PlayerRatingStatsDTO> Leaderboard { get; set; } = [];
}
