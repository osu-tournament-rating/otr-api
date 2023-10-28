namespace API.DTOs;

public class PlayerStatsDTO
{
	public PlayerStatsDTO(BaseStatsDTO? generalStats, AggregatePlayerMatchStatsDTO? matchStats, PlayerScoreStatsDTO? scoreStats, 
		PlayerTournamentStatsDTO? tournamentStats, IEnumerable<MatchRatingStatsDTO> ratingStats)
	{
		GeneralStats = generalStats;
		MatchStats = matchStats;
		ScoreStats = scoreStats;
		TournamentStats = tournamentStats;
		RatingStats = ratingStats;
	}

	public BaseStatsDTO? GeneralStats { get; }
	public AggregatePlayerMatchStatsDTO? MatchStats { get; }
	public PlayerScoreStatsDTO? ScoreStats { get; }
	public PlayerTournamentStatsDTO? TournamentStats { get; }
	public IEnumerable<MatchRatingStatsDTO> RatingStats { get; }
}