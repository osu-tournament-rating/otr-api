namespace API.DTOs;

public class PlayerStatisticsDTO
{
	public PlayerStatisticsDTO(AggregatePlayerMatchStatisticsDTO? matchStatistics, PlayerScoreStatsDTO? scoreStatistics, 
		PlayerTournamentStatsDTO? tournamentStatistics, IEnumerable<MatchRatingStatisticsDTO> ratingStatistics)
	{
		MatchStatistics = matchStatistics;
		ScoreStatistics = scoreStatistics;
		TournamentStatistics = tournamentStatistics;
		RatingStatistics = ratingStatistics;
	}

	public AggregatePlayerMatchStatisticsDTO? MatchStatistics { get; }
	public PlayerScoreStatsDTO? ScoreStatistics { get; }
	public PlayerTournamentStatsDTO? TournamentStatistics { get; }
	public IEnumerable<MatchRatingStatisticsDTO> RatingStatistics { get; }
}