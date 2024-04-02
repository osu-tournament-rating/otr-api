namespace API.DTOs;

/// <summary>
/// Represents a collection of statistics for a player
/// </summary>
public class PlayerStatsDTO(
    PlayerInfoDTO? playerInfo,
    BaseStatsDTO? generalStats,
    AggregatePlayerMatchStatsDTO? matchStats,
    PlayerModStatsDTO? modStats,
    PlayerTournamentStatsDTO? tournamentStats,
    PlayerRatingChartDTO ratingChart,
    IEnumerable<PlayerFrequencyDTO> frequentTeammates,
    IEnumerable<PlayerFrequencyDTO> frequentOpponents
    )
{
    /// <summary>
    /// Player info
    /// </summary>
    public PlayerInfoDTO? PlayerInfo { get; set; } = playerInfo;

    /// <summary>
    /// General stats for the player
    /// </summary>
    public BaseStatsDTO? GeneralStats { get; } = generalStats;

    /// <summary>
    /// Match stats for the player
    /// </summary>
    public AggregatePlayerMatchStatsDTO? MatchStats { get; } = matchStats;

    /// <summary>
    /// Mod stats for the player
    /// </summary>
    public PlayerModStatsDTO? ModStats { get; } = modStats;

    /// <summary>
    /// Tournament participation and performance stats for the player
    /// </summary>
    public PlayerTournamentStatsDTO? TournamentStats { get; } = tournamentStats;

    /// <summary>
    /// List of frequencies of the player's teammates
    /// </summary>
    public IEnumerable<PlayerFrequencyDTO> FrequentTeammates { get; } = frequentTeammates;

    /// <summary>
    /// List of frequencies of the player's opponents
    /// </summary>
    public IEnumerable<PlayerFrequencyDTO> FrequentOpponents { get; } = frequentOpponents;

    /// <summary>
    /// Rating chart for the player
    /// </summary>
    public PlayerRatingChartDTO RatingChart { get; } = ratingChart;
}
