using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using APITests.SeedData;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockPlayerMatchStatsRepository : Mock<IPlayerMatchStatsRepository>
{
    public MockPlayerMatchStatsRepository()
    {
        SetupAll();
    }

    public MockPlayerMatchStatsRepository SetupAll() =>
        SetupGet()
            .SetupTeammateStats()
            .SetupOpponentStats()
            .SetupGetModStats()
            .SetupCountMatchesPlayed()
            .SetupCountMatchesWon()
            .SetupGlobalWinrate();

    public MockPlayerMatchStatsRepository SetupGet()
    {
        Setup(x => x.GetAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()
            )
        ).ReturnsAsync(() =>
        {
            var matchStats = new List<PlayerMatchStats>();
            for (var i = 0; i < 10; i++)
            {
                matchStats.Add(SeededPlayerMatchStats.Get());
            }

            return matchStats;
        });

        return this;
    }

    public MockPlayerMatchStatsRepository SetupTeammateStats()
    {
        Setup(x => x.TeammateStatsAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()
            )
        ).ReturnsAsync((int playerId, int _, int _, DateTime _, DateTime _) =>
        {
            var matchStats = new List<PlayerMatchStats>();
            for (var i = 0; i < 3; i++)
            {
                PlayerMatchStats teammate = SeededPlayerMatchStats.Get();
                teammate.PlayerId = playerId + Random.Shared.Next() % 5000;

                matchStats.Add(teammate);
            }

            return matchStats;
        });

        return this;
    }

    public MockPlayerMatchStatsRepository SetupOpponentStats()
    {
        Setup(x => x.OpponentStatsAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()
            )
        ).ReturnsAsync((int playerId, int _, int _, DateTime _, DateTime _) =>
        {
            var matchStats = new List<PlayerMatchStats>();
            for (var i = 0; i < 4; i++)
            {
                PlayerMatchStats teammate = SeededPlayerMatchStats.Get();
                teammate.PlayerId = playerId + Random.Shared.Next() % 5000;

                matchStats.Add(teammate);
            }

            return matchStats;
        });

        return this;
    }

    public MockPlayerMatchStatsRepository SetupGetModStats()
    {
        Setup(x => x.GetModStatsAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()
            )
        ).ReturnsAsync(
            new PlayerModStatsDTO
            {
                PlayedNM = SeededModStatsDTO.Get(),
                PlayedEZ = null,
                PlayedHT = null,
                PlayedHD = SeededModStatsDTO.Get(),
                PlayedHR = SeededModStatsDTO.Get(),
                PlayedDT = SeededModStatsDTO.Get(),
                PlayedFL = null,
                PlayedHDHR = SeededModStatsDTO.Get(),
                PlayedHDDT = null,
                PlayedHDEZ = null
            });

        return this;
    }

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

    public MockPlayerMatchStatsRepository SetupCountMatchesWon()
    {
        Setup(x =>
                x.CountMatchesWonAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()
                )
            )
            .ReturnsAsync(500);

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
