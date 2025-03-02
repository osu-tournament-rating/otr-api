using Common.Enums.Enums;
using Database.Repositories.Interfaces;
using Moq;

namespace APITests.MockRepositories;

public class MockTournamentsRepository : Mock<ITournamentsRepository>
{
    public MockTournamentsRepository SetupCountPlayed()
    {
        Setup(x =>
                x.CountPlayedAsync(
                    It.IsAny<int>(),
                    It.IsAny<Ruleset>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()
                )
            )
            .ReturnsAsync(3);

        return this;
    }
}
