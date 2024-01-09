namespace API.DTOs;

public class PlayerRankChartDTO
{
	public IEnumerable<RankChartDataPointDTO> ChartData { get; set; } = new List<RankChartDataPointDTO>();
}