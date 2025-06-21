using Common.Enums;
using JetBrains.Annotations;

namespace API.DTOs;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class LeaderboardDTO
{
    /// <summary>
    /// The maximum page count for which there will be results
    /// </summary>
    public int Pages { get; set; }
    public Ruleset Ruleset { get; set; }
    public IEnumerable<PlayerRatingStatsDTO> Leaderboard { get; set; } = [];
}
