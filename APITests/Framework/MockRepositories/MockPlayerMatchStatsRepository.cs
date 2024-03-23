using API.Repositories.Interfaces;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockPlayerMatchStatsRepository : Mock<IPlayerMatchStatsRepository>
{
    public MockPlayerMatchStatsRepository SetupCountMatchesPlayed()
    {
        Setup(x =>
                x.CountMatchesPlayedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync(1000);

        return this;
    }

    public MockPlayerMatchStatsRepository SetupGlobalWinrate()
    {
        Setup(x =>
                x.GlobalWinrateAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync(0.75);

        return this;
    }
}
