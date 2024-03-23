using API.Entities;
using API.Repositories.Interfaces;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockGameWinRecordsRepository : Mock<IGameWinRecordsRepository>
{
    public MockGameWinRecordsRepository()
    {
        SetupAll();
    }

    private MockGameWinRecordsRepository SetupAll() =>
        SetupGet();

    private MockGameWinRecordsRepository SetupGet()
    {
        Setup(x => x.GetAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                GameWinRecord gameWinRecord = new()
                {
                    Id = 123,
                    GameId = 28397,
                    Winners =
                    [
                        id,
                        2,
                        3,
                        4
                    ],
                    Losers =
                    [
                        5,
                        6,
                        7,
                        8
                    ],
                    WinnerTeam = 2,
                    LoserTeam = 1
                };

                return gameWinRecord;
            });

        return this;
    }
}
