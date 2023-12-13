namespace API.DTOs;

public class PlayerStatsDTO
{
	public PlayerStatsDTO(BaseStatsDTO? generalStats, AggregatePlayerMatchStatsDTO? matchStats, PlayerModStatsDTO? modStats,
		PlayerTournamentStatsDTO? tournamentStats, IEnumerable<MatchRatingStatsDTO> ratingStats, PlayerTeammateComparisonDTO? teammateComparison,
		PlayerOpponentComparisonDTO? opponentComparison)
	{
		GeneralStats = generalStats;
		MatchStats = matchStats;
		ModStats = modStats;
		TournamentStats = tournamentStats;
		RatingStats = ratingStats;
		TeammateComparison = teammateComparison;
		OpponentComparison = opponentComparison;
	}

	public BaseStatsDTO? GeneralStats { get; }
	public AggregatePlayerMatchStatsDTO? MatchStats { get; }
	public PlayerModStatsDTO? ModStats { get; }
	public PlayerTournamentStatsDTO? TournamentStats { get; }
	public PlayerTeammateComparisonDTO? TeammateComparison { get; }
	public PlayerOpponentComparisonDTO? OpponentComparison { get; }
	public IEnumerable<MatchRatingStatsDTO> RatingStats { get; }
}