using API.Entities;
using API.Repositories.Interfaces;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockMatchDuplicateRepository : Mock<IMatchDuplicateRepository>
{
    public MockMatchDuplicateRepository()
    {
        SetupAll();
    }

    public MockMatchDuplicateRepository SetupAll() =>
        SetupGetDuplicates()
            .SetupGetAllUnverified();

    public MockMatchDuplicateRepository SetupGetDuplicates()
    {
        Setup(x => x.GetDuplicatesAsync(It.IsAny<int>()))
            .ReturnsAsync((int matchId) => new List<MatchDuplicate>
            {
                new()
                {
                    Id = 1,
                    MatchId = 123,
                    SuspectedDuplicateOf = matchId,
                    OsuMatchId = 4534535,
                    VerifiedAsDuplicate = false,
                    VerifiedBy = null,
                    Verifier = null
                },
                new()
                {
                    Id = 2,
                    MatchId = 124,
                    SuspectedDuplicateOf = matchId,
                    OsuMatchId = 0,
                    VerifiedAsDuplicate = false,
                    VerifiedBy = null,
                    Verifier = null
                }
            });

        return this;
    }

    public MockMatchDuplicateRepository SetupGetAllUnverified()
    {
        Setup(x => x.GetAllUnverifiedAsync())
            .ReturnsAsync((int matchId) => new List<MatchDuplicate>
            {
                new()
                {
                    Id = 1,
                    MatchId = 123,
                    SuspectedDuplicateOf = matchId,
                    OsuMatchId = 4534535,
                    VerifiedAsDuplicate = false,
                    VerifiedBy = null,
                    Verifier = null
                },
                new()
                {
                    Id = 2,
                    MatchId = 124,
                    SuspectedDuplicateOf = matchId,
                    OsuMatchId = 0,
                    VerifiedAsDuplicate = false,
                    VerifiedBy = null,
                    Verifier = null
                }
            });

        return this;
    }
}
