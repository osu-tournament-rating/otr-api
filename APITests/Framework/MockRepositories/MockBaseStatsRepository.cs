using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using APITests.SeedData;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockBaseStatsRepository : Mock<IBaseStatsRepository>
{
    public MockBaseStatsRepository()
    {
        SetupAll();
    }

    public MockBaseStatsRepository SetupAll() =>
            SetupGet()
            .SetupGetGlobalRank()
            .SetupGetRecentCreatedDate()
            .SetupLeaderboard()
            .SetupHighestRating()
            .SetupHighestMatches()
            .SetupGetHistogramAsync()
            .SetupHighestRank()
            .SetupGetForPlayerAsync();

    public MockBaseStatsRepository SetupGet()
    {
        Setup(x =>
                x.GetAsync(It.IsAny<long>())
            )
            .ReturnsAsync(new[] { SeededBaseStats.Get() });

        Setup(x =>
                x.GetAsync(It.IsAny<int>(), It.IsAny<int>()
            )).ReturnsAsync(SeededBaseStats.Get());

        return this;
    }

    public MockBaseStatsRepository SetupGetGlobalRank()
    {
        Setup(x =>
                x.GetGlobalRankAsync(It.IsAny<long>(), It.IsAny<int>())
            )
            .ReturnsAsync(20); // Some global rank

        return this;
    }

    public MockBaseStatsRepository SetupGetRecentCreatedDate()
    {
        Setup(x =>
                x.GetRecentCreatedDate(It.IsAny<long>())
            )
            .ReturnsAsync(new DateTime(2023, 11, 11));

        return this;
    }

    public MockBaseStatsRepository SetupLeaderboard()
    {
        Setup(x =>
                x.GetLeaderboardAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<LeaderboardChartType>(),
                    It.IsAny<LeaderboardFilterDTO>(),
                    It.IsAny<int?>()
                )
            )
            .ReturnsAsync(
                (int _, int pageSize, int _, LeaderboardChartType _, LeaderboardFilterDTO filter, int? _) =>
                    SeededBaseStats.GetLeaderboardFiltered(filter, pageSize)
            );

        Setup(x =>
                x.LeaderboardCountAsync(
                    It.IsAny<int>(),
                    LeaderboardChartType.Global,
                    new LeaderboardFilterDTO(),
                    null
                )
            )
            .ReturnsAsync(10000);

        return this;
    }

    public MockBaseStatsRepository SetupHighestRating()
    {
        Setup(x => x.HighestRatingAsync(It.IsAny<int>(), It.IsAny<string?>())).ReturnsAsync(3200);

        return this;
    }

    public MockBaseStatsRepository SetupHighestMatches()
    {
        Setup(x => x.HighestMatchesAsync(It.IsAny<int>(), It.IsAny<string?>())).ReturnsAsync(500);

        return this;
    }

    public MockBaseStatsRepository SetupGetHistogramAsync()
    {
        Setup(x => x.GetHistogramAsync(It.IsAny<int>()))
            .ReturnsAsync(new Dictionary<int, int>
            {
                // Left = rating, Right = count
                { 100, 0 },
                { 125, 100 },
                { 150, 200 },
                { 175, 300 },
                { 200, 400 },
                { 225, 500 },
                { 250, 600 },
                { 275, 700 },
                { 300, 800 },
                { 325, 900 },
                { 350, 1000 }
            });

        return this;
    }

    public MockBaseStatsRepository SetupHighestRank()
    {
        Setup(x => x.HighestRankAsync(It.IsAny<int>(), It.IsAny<string?>())).ReturnsAsync(100_000_000);

        return this;
    }

    public MockBaseStatsRepository SetupGetForPlayerAsync()
    {
        Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(SeededBaseStats.Get());

        return this;
    }
}
