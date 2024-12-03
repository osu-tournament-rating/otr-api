using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using APITests.SeedData;
using Database.Enums;
using Moq;

namespace APITests.MockRepositories;

public class MockPlayerRatingsRepository : Mock<IApiPlayerRatingsRepository>
{
    public MockPlayerRatingsRepository SetupLeaderboardCount()
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

    public MockPlayerRatingsRepository SetupLeaderboard()
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
                    SeededPlayerRatings.GetLeaderboardFiltered(filter, pageSize)
            );

        return this;
    }

    public MockPlayerRatingsRepository SetupHighestRating()
    {
        Setup(x => x.HighestRatingAsync(It.IsAny<Ruleset>(), It.IsAny<string?>())).ReturnsAsync(3200);

        return this;
    }

    public MockPlayerRatingsRepository SetupHighestMatches()
    {
        Setup(x => x.HighestMatchesAsync(It.IsAny<Ruleset>(), It.IsAny<string?>())).ReturnsAsync(500);

        return this;
    }

    public MockPlayerRatingsRepository SetupHighestRank()
    {
        Setup(x => x.HighestRankAsync(It.IsAny<Ruleset>(), It.IsAny<string?>())).ReturnsAsync(100_000_000);

        return this;
    }

    public MockPlayerRatingsRepository SetupGetForPlayerAsync()
    {
        Setup(x => x.GetForPlayerAsync(It.IsAny<int>(), It.IsAny<Ruleset>())).ReturnsAsync(SeededPlayerRatings.Get());

        return this;
    }
}
