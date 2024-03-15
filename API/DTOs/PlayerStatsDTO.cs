namespace API.DTOs;

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
    public PlayerInfoDTO? PlayerInfo { get; set; } = playerInfo;
    public BaseStatsDTO? GeneralStats { get; } = generalStats;
    public AggregatePlayerMatchStatsDTO? MatchStats { get; } = matchStats;
    public PlayerModStatsDTO? ModStats { get; } = modStats;
    public PlayerTournamentStatsDTO? TournamentStats { get; } = tournamentStats;
    public IEnumerable<PlayerFrequencyDTO> FrequentTeammates { get; } = frequentTeammates;
    public IEnumerable<PlayerFrequencyDTO> FrequentOpponents { get; } = frequentOpponents;
    public PlayerRatingChartDTO RatingChart { get; } = ratingChart;
}
