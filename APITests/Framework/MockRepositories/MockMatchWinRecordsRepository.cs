using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockMatchWinRecordsRepository : Mock<IMatchWinRecordsRepository>
{
    public MockMatchWinRecordsRepository()
    {
        SetupAll();
    }

    public MockMatchWinRecordsRepository SetupAll() =>
        SetupGet()
            .SetupGetFrequentTeammates()
            .SetupGetFrequentOpponents();

    public MockMatchWinRecordsRepository SetupGet()
    {
        Setup(x =>
                x.GetAsync(It.IsAny<int>())
            )
            .ReturnsAsync((int id) =>
            {
                MatchWinRecord matchWinRecord = new()
                {
                    Id = id,
                    MatchId = 28397,
                    WinnerTeam = 2,
                    LoserTeam = 1
                };

                return matchWinRecord;
            });

        return this;
    }

    public MockMatchWinRecordsRepository SetupGetFrequentTeammates()
    {
        Setup(x => x.GetFrequentTeammatesAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(), It.IsAny<int>()))
            .ReturnsAsync((int playerId, int mode, DateTime? dateMin, DateTime? dateMax, int limit) =>
            {
                var list = new List<PlayerFrequencyDTO>();

                for (var i = 0; i < limit; i++)
                {
                    list.Add(new PlayerFrequencyDTO
                    {
                        PlayerId = Random.Shared.Next() % 10000,
                        OsuId = Random.Shared.Next() % 1000000,
                        Username = "TestUser",
                        Frequency = Random.Shared.Next() % 100
                    });
                }

                return list;
            });

        return this;
    }

    public MockMatchWinRecordsRepository SetupGetFrequentOpponents()
    {
        Setup(x => x.GetFrequentOpponentsAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(), It.IsAny<int>()))
            .ReturnsAsync((int playerId, int mode, DateTime? dateMin, DateTime? dateMax, int limit) =>
            {
                var list = new List<PlayerFrequencyDTO>();

                for (var i = 0; i < limit; i++)
                {
                    list.Add(new PlayerFrequencyDTO
                    {
                        PlayerId = Random.Shared.Next() % 10000,
                        OsuId = Random.Shared.Next() % 1000000,
                        Username = "TestUser",
                        Frequency = Random.Shared.Next() % 100
                    });
                }

                return list;
            });

        return this;
    }
}
