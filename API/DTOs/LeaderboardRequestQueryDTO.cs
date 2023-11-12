using API.Enums;

namespace API.DTOs;

public class LeaderboardRequestQueryDTO
{
	public int Mode { get; set; }
	public int Page { get; set; }
	public int PageSize { get; set; } = 50;
	public int? PlayerId { get; set; }
	public LeaderboardChartType ChartType { get; set; } = LeaderboardChartType.Global;
	public LeaderboardFilterDTO Filter { get; set; } = new();
}