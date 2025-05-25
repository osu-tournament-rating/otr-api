namespace API.DTOs;

public class PlayerTournamentStatsDTO : PlayerTournamentStatsBaseDTO
{
    /// <summary>
    /// The tournament that these stats are for
    /// </summary>
    public TournamentCompactDTO Tournament { get; set; } = null!;
}
