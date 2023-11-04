namespace API.DTOs;

public class PlayerStatsDTO
{
	public PlayerStatsDTO(BaseStatsDTO? generalStats, AggregatePlayerMatchStatsDTO? matchStats, PlayerScoreStatsDTO? scoreStats, 
		PlayerTournamentStatsDTO? tournamentStats, IEnumerable<MatchRatingStatsDTO> ratingStats, PlayerTeammateComparisonDTO? teammateComparison,
		PlayerOpponentComparisonDTO? opponentComparison)
	{
		GeneralStats = generalStats;
		MatchStats = matchStats;
		ScoreStats = scoreStats;
		TournamentStats = tournamentStats;
		RatingStats = ratingStats;
		TeammateComparison = teammateComparison;
		OpponentComparison = opponentComparison;
	}

	public BaseStatsDTO? GeneralStats { get; }
	public AggregatePlayerMatchStatsDTO? MatchStats { get; }
	public PlayerScoreStatsDTO? ScoreStats { get; }
	public PlayerTournamentStatsDTO? TournamentStats { get; }
	public PlayerTeammateComparisonDTO? TeammateComparison { get; }
	public PlayerOpponentComparisonDTO? OpponentComparison { get; }
	public IEnumerable<MatchRatingStatsDTO> RatingStats { get; }
}