using Database.Enums;
using Database.Repositories.Interfaces;
using Moq;

namespace APITests.MockRepositories;

public class MockMatchStatsRepository : Mock<IPlayerMatchStatsRepository>
{
    public MockMatchStatsRepository SetupCountMatchesPlayed()
    {
        Setup(x =>
                x.CountMatchesPlayedAsync(
                    It.IsAny<int>(),
                    It.IsAny<Ruleset>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync(1000);

        return this;
    }

    public MockMatchStatsRepository SetupGlobalWinRate()
    {
        Setup(x =>
                x.GlobalWinrateAsync(
                    It.IsAny<int>(),
                    It.IsAny<Ruleset>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync(0.75);

        return this;
    }
}
