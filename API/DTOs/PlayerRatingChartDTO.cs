namespace API.DTOs;

public class PlayerRatingChartDTO
{
    public IEnumerable<IEnumerable<PlayerRatingChartDataPointDTO>> ChartData { get; set; } =
        new List<List<PlayerRatingChartDataPointDTO>>();
}
