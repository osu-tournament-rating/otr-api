namespace API.DTOs;

public class PlayerStatisticsDTO
{
	public PlayerStatisticsDTO(PlayerMatchStatisticsDTO? matchStatistics, PlayerScoreStatsDTO? scoreStatistics)
	{
		MatchStatistics = matchStatistics;
		ScoreStatistics = scoreStatistics;
	}

	public PlayerMatchStatisticsDTO? MatchStatistics { get; set; }
	public PlayerScoreStatsDTO? ScoreStatistics { get; set; }
}