namespace API.DTOs;

public class PlayerStatsDTO
{
	public PlayerStatsDTO(BaseStatsDTO? generalStats, AggregatePlayerMatchStatsDTO? matchStats, PlayerModStatsDTO? modStats,
		PlayerTournamentStatsDTO? tournamentStats, IEnumerable<MatchRatingStatsDTO> ratingStats, IEnumerable<PlayerFrequencyDTO> frequentTeammates,
		IEnumerable<PlayerFrequencyDTO> frequentOpponents)
	{
		GeneralStats = generalStats;
		MatchStats = matchStats;
		ModStats = modStats;
		TournamentStats = tournamentStats;
		RatingStats = ratingStats;
		FrequentTeammates = frequentTeammates;
		FrequentOpponents = frequentOpponents;
	}

	public BaseStatsDTO? GeneralStats { get; }
	public AggregatePlayerMatchStatsDTO? MatchStats { get; }
	public PlayerModStatsDTO? ModStats { get; }
	public PlayerTournamentStatsDTO? TournamentStats { get; }
	public IEnumerable<PlayerFrequencyDTO> FrequentTeammates { get; }
	public IEnumerable<PlayerFrequencyDTO> FrequentOpponents { get; }
	public IEnumerable<MatchRatingStatsDTO> RatingStats { get; }
}