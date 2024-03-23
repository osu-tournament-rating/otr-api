using API.Enums;
using API.Repositories.Interfaces;
using APITests.SeedData.DTOs;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockRatingStatsRepository : Mock<IMatchRatingStatsRepository>
{
    public MockRatingStatsRepository SetupHighestGlobalRank()
    {
        Setup(x =>
                x.HighestGlobalRankAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync(3000);

        return this;
    }

    public MockRatingStatsRepository SetupHighestCountryRank()
    {
        Setup(x =>
                x.HighestCountryRankAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync(750);

        return this;
    }

    public MockRatingStatsRepository SetupGetRankChart()
    {
        Setup(x =>
                x.GetRankChartAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<LeaderboardChartType>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync(SeededPlayerRankChartDTO.Get());

        return this;
    }
}
