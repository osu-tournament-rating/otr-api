using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using APITests.SeedData;
using Database.Enums;
using Moq;

namespace APITests.MockRepositories;

public class MockBaseStatsRepository : Mock<IApiPlayerRatingsRepository>
{
    public MockBaseStatsRepository SetupLeaderboardCount()
    {
        Setup(x =>
                x.LeaderboardCountAsync(
                    It.IsAny<Ruleset>(),
                    LeaderboardChartType.Global,
                    new LeaderboardFilterDTO(),
                    null
                )
            )
            .ReturnsAsync(10000);

        return this;
    }

    public MockBaseStatsRepository SetupLeaderboard()
    {
        Setup(x =>
                x.GetLeaderboardAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Ruleset>(),
                    It.IsAny<LeaderboardChartType>(),
                    It.IsAny<LeaderboardFilterDTO>(),
                    It.IsAny<int?>()
                )
            )
            .ReturnsAsync(
                (int _, int pageSize, int _, LeaderboardChartType _, LeaderboardFilterDTO filter, int? _) =>
                    SeededBaseStats.GetLeaderboardFiltered(filter, pageSize)
            );

        return this;
    }

    public MockBaseStatsRepository SetupHighestRating()
    {
        Setup(x => x.HighestRatingAsync(It.IsAny<Ruleset>(), It.IsAny<string?>())).ReturnsAsync(3200);

        return this;
    }

    public MockBaseStatsRepository SetupHighestMatches()
    {
        Setup(x => x.HighestMatchesAsync(It.IsAny<Ruleset>(), It.IsAny<string?>())).ReturnsAsync(500);

        return this;
    }

    public MockBaseStatsRepository SetupHighestRank()
    {
        Setup(x => x.HighestRankAsync(It.IsAny<Ruleset>(), It.IsAny<string?>())).ReturnsAsync(100_000_000);

        return this;
    }

    public MockBaseStatsRepository SetupGetForPlayerAsync()
    {
        Setup(x => x.GetForPlayerAsync(It.IsAny<int>(), It.IsAny<Ruleset>())).ReturnsAsync(SeededBaseStats.Get());

        return this;
    }
}
