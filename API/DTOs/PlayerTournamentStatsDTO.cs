using JetBrains.Annotations;

namespace API.DTOs;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class PlayerTournamentStatsDTO : PlayerTournamentStatsBaseDTO
{
    /// <summary>
    /// The tournament that these stats are for
    /// </summary>
    public TournamentCompactDTO Tournament { get; set; } = null!;
}
