namespace API.DTOs;

/// <summary>
/// Represents data used to construct a rating delta chart for a player
/// </summary>
public class PlayerRatingChartDTO
{
    /// <summary>
    /// List of data points used to construct the chart
    /// </summary>
    public IEnumerable<IEnumerable<PlayerRatingChartDataPointDTO>> ChartData { get; set; } =
        new List<List<PlayerRatingChartDataPointDTO>>();
}
