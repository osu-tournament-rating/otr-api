using API.DTOs;

namespace APITests.SeedData.DTOs;

public static class SeededPlayerRankChartDTO
{
    public static PlayerRankChartDTO Get()
    {
        var chart = new PlayerRankChartDTO();
        var chartData = new List<RankChartDataPointDTO>();

        for (var i = 0; i < 25; i++)
        {
            chartData.Add(
                new RankChartDataPointDTO
                {
                    TournamentName = "FooBar Tournament",
                    MatchName = "FB: (Foo) vs (Bar)",
                    Rank = 500,
                    RankChange = 12
                }
            );
        }

        chart.ChartData = chartData;
        return chart;
    }
}
