namespace API.DTOs;

public class PlayerStatsDTO
{
	public PlayerStatsDTO(PlayerInfoDTO? playerInfo, BaseStatsDTO? generalStats, AggregatePlayerMatchStatsDTO? matchStats, PlayerModStatsDTO? modStats,
		PlayerTournamentStatsDTO? tournamentStats, PlayerRatingChartDTO ratingChart, IEnumerable<PlayerFrequencyDTO> frequentTeammates,
		IEnumerable<PlayerFrequencyDTO> frequentOpponents)
	{
		PlayerInfo = playerInfo;
		GeneralStats = generalStats;
		MatchStats = matchStats;
		ModStats = modStats;
		TournamentStats = tournamentStats;
		RatingChart = ratingChart;
		FrequentTeammates = frequentTeammates;
		FrequentOpponents = frequentOpponents;
	}

	public PlayerInfoDTO? PlayerInfo { get; set; }
	public BaseStatsDTO? GeneralStats { get; }
	public AggregatePlayerMatchStatsDTO? MatchStats { get; }
	public PlayerModStatsDTO? ModStats { get; }
	public PlayerTournamentStatsDTO? TournamentStats { get; }
	public IEnumerable<PlayerFrequencyDTO> FrequentTeammates { get; }
	public IEnumerable<PlayerFrequencyDTO> FrequentOpponents { get; }
	public PlayerRatingChartDTO RatingChart { get; }
}