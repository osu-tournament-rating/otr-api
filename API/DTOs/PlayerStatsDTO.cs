namespace API.DTOs;

public class PlayerStatsDTO
{
	public PlayerStatsDTO(BaseStatsDTO? generalStats, AggregatePlayerMatchStatsDTO? matchStats, PlayerModStatsDTO? modStats, PlayerScoreStatsDTO? scoreStats,
		PlayerTournamentStatsDTO? tournamentStats, IEnumerable<MatchRatingStatsDTO> ratingStats, PlayerTeammateComparisonDTO? teammateComparison,
		PlayerOpponentComparisonDTO? opponentComparison)
	{
		GeneralStats = generalStats;
		MatchStats = matchStats;
		ModStats = modStats;
		ScoreStats = scoreStats;
		TournamentStats = tournamentStats;
		RatingStats = ratingStats;
		TeammateComparison = teammateComparison;
		OpponentComparison = opponentComparison;
	}

	public BaseStatsDTO? GeneralStats { get; }
	public AggregatePlayerMatchStatsDTO? MatchStats { get; }
	public PlayerModStatsDTO? ModStats { get; }
	public PlayerScoreStatsDTO? ScoreStats { get; }
	public PlayerTournamentStatsDTO? TournamentStats { get; }
	public PlayerTeammateComparisonDTO? TeammateComparison { get; }
	public PlayerOpponentComparisonDTO? OpponentComparison { get; }
	public IEnumerable<MatchRatingStatsDTO> RatingStats { get; }
}