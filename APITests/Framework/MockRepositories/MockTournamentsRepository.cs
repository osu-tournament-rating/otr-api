using API.Repositories.Interfaces;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockTournamentsRepository : Mock<ITournamentsRepository>
{
    public MockTournamentsRepository SetupCountPlayed()
    {
        Setup(x =>
                x.CountPlayedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync(3);

        return this;
    }
}
