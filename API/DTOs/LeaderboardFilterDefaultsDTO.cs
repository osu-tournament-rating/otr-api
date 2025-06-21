using JetBrains.Annotations;

namespace API.DTOs;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class LeaderboardFilterDefaultsDTO
{
    public int MaxRank { get; set; }
    public double MaxRating { get; set; }
    public int MaxMatches { get; set; }
}
