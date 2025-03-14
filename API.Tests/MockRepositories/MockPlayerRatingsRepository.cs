using APITests.SeedData;
using Common.Enums;
using Database.Entities.Processor;
using Database.Models;
using Database.Repositories.Interfaces;
using Moq;

namespace APITests.MockRepositories;

public class MockPlayerRatingsRepository : Mock<IPlayerRatingsRepository>
{
    public MockPlayerRatingsRepository SetupLeaderboardCount()
    {
        Setup(x =>
                x.LeaderboardCountAsync(
                    It.IsAny<Ruleset>(),
                    LeaderboardChartType.Global,
                    new LeaderboardFilter(),
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
                    It.IsAny<LeaderboardFilter>(),
                    It.IsAny<string?>()
                )
            )
            .ReturnsAsync(
                (int _, int pageSize, int _, LeaderboardChartType _, LeaderboardFilter filter, string? _) =>
                    SeededPlayerRatings.GetLeaderboardFiltered(filter, pageSize)
            );

        return this;
    }

    public MockPlayerRatingsRepository SetupGetAsync()
    {
        Setup(x =>
                x.GetAsync(It.IsAny<int>(), It.IsAny<Ruleset>(),
                    It.IsAny<bool>()))
            .ReturnsAsync(
                (int playerId, Ruleset ruleset, bool includeAdjustments) =>
                    new PlayerRating
                    {
                        PlayerId = playerId,
                        Ruleset = ruleset,
                        Adjustments = includeAdjustments
                            ?
                            [
                                new RatingAdjustment
                                {
                                    AdjustmentType = RatingAdjustmentType.Initial,
                                    Ruleset = ruleset,
                                    RatingBefore = 0,
                                    RatingAfter = 1200,
                                    VolatilityBefore = 0,
                                    VolatilityAfter = 300,
                                    PlayerId = playerId,
                                }
                            ]
                            : []
                    }
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
        Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<Ruleset>(), true)).ReturnsAsync(SeededPlayerRatings.Get());

        return this;
    }
}
