namespace API.DTOs;

public class PlayerRankChartDTO
{
	// Grouped by day, 1 list per day
	public IEnumerable<RankChartDataPointDTO> ChartData { get; set; } = new List<RankChartDataPointDTO>();
}