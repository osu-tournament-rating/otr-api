using API.Enums;
using API.Repositories.Interfaces;
using APITests.SeedData;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockMatchesRepository : Mock<IMatchesRepository>
{
    public MockMatchesRepository()
    {
        SetupAll();
    }

    public MockMatchesRepository SetupAll() =>
        SetupGet()
            .SetupGetAll()
            .SetupGetByMatchId()
            .SetupGetMatchesNeedingAutoCheck()
            .GetFirstMatchNeedingApiProcessing()
            .GetFirstMatchNeedingAutoCheck()
            .GetMatchesNeedingApiProcessing()
            .SetupUpdateVerificationStatus();

    public MockMatchesRepository SetupGet()
    {
        Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(SeededMatch.Generate(123, 1).First());

        Setup(x => x.GetAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync((IEnumerable<long> osuIds) => SeededMatch.Generate(osuIds));

        Setup(x => x.GetAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<bool>()))
            .ReturnsAsync((IEnumerable<int> ids) => SeededMatch.Generate(ids));

        return this;
    }

    public MockMatchesRepository SetupGetAll()
    {
        Setup(x => x.GetAllAsync())
            .ReturnsAsync(SeededMatch.Generate(123, 32));

        return this;
    }

    public MockMatchesRepository SetupGetByMatchId()
    {
        Setup(x => x.GetByMatchIdAsync(It.IsAny<long>()))
            .ReturnsAsync((long matchId) => SeededMatch.Generate(new List<long> { matchId }).First());

        return this;
    }

    public MockMatchesRepository SetupGetMatchesNeedingAutoCheck()
    {
        Setup(x => x.GetMatchesNeedingAutoCheckAsync())
            .ReturnsAsync(SeededMatch.Generate(123, 32).ToList());

        return this;
    }

    public MockMatchesRepository GetFirstMatchNeedingApiProcessing()
    {
        Setup(x => x.GetFirstMatchNeedingApiProcessingAsync())
            .ReturnsAsync(SeededMatch.Generate(123, 1).First());

        return this;
    }

    public MockMatchesRepository GetFirstMatchNeedingAutoCheck()
    {
        Setup(x => x.GetFirstMatchNeedingAutoCheckAsync())
            .ReturnsAsync(SeededMatch.Generate(123, 1).First());

        return this;
    }

    public MockMatchesRepository GetMatchesNeedingApiProcessing()
    {
        Setup(x => x.GetMatchesNeedingAutoCheckAsync())
            .ReturnsAsync(SeededMatch.Generate(123, 32).ToList());

        return this;
    }

    public MockMatchesRepository SetupUpdateVerificationStatus()
    {
        Setup(x => x.UpdateVerificationStatusAsync(It.IsAny<long>(), It.IsAny<MatchVerificationStatus>(),
                It.IsAny<MatchVerificationSource>(), It.IsAny<string?>()))
            .ReturnsAsync(1);

        return this;
    }

    // Todo: Add the rest of the methods as needed during testing...too much to do right now!
}
