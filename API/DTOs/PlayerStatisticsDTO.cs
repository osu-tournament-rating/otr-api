namespace API.DTOs;

public class PlayerStatisticsDTO
{
	public PlayerStatisticsDTO(AggregatePlayerMatchStatisticsDTO? matchStatistics, PlayerScoreStatsDTO? scoreStatistics)
	{
		MatchStatistics = matchStatistics;
		ScoreStatistics = scoreStatistics;
	}

	public AggregatePlayerMatchStatisticsDTO? MatchStatistics { get; set; }
	public PlayerScoreStatsDTO? ScoreStatistics { get; set; }
}