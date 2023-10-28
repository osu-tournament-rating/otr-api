namespace API.DTOs;

public class PlayerStatisticsDTO
{
	public PlayerStatisticsDTO(AggregatePlayerMatchStatisticsDTO? matchStatistics, PlayerScoreStatsDTO? scoreStatistics, PlayerTournamentStatsDTO? tournamentStatistics)
	{
		MatchStatistics = matchStatistics;
		ScoreStatistics = scoreStatistics;
		TournamentStatistics = tournamentStatistics;
	}

	public AggregatePlayerMatchStatisticsDTO? MatchStatistics { get; }
	public PlayerScoreStatsDTO? ScoreStatistics { get; }
	public PlayerTournamentStatsDTO? TournamentStatistics { get; }
}