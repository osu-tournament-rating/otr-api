using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using APITests.SeedData;
using Moq;

namespace APITests.MockRepositories;

public class MockBaseStatsRepository : Mock<IBaseStatsRepository>
{
	public MockBaseStatsRepository SetupLeaderboardCount()
	{
		Setup(x => x.LeaderboardCountAsync(It.IsAny<int>(), LeaderboardChartType.Global, new LeaderboardFilterDTO(), null))
			.ReturnsAsync(10000);

		return this;
	}

	public MockBaseStatsRepository SetupLeaderboard()
	{
		Setup(x => x.GetLeaderboardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<LeaderboardChartType>(), It.IsAny<LeaderboardFilterDTO>(),
				It.IsAny<int?>()))
			.ReturnsAsync((int page, int pageSize, int mode, LeaderboardChartType chartType,
					LeaderboardFilterDTO filter, int? playerId) =>
				SeededBaseStats.GetLeaderboardFiltered(filter, pageSize));

		return this;
	}

	public MockBaseStatsRepository SetupHighestRating()
	{
		Setup(x => x.HighestRatingAsync(It.IsAny<int>(), It.IsAny<string?>())).ReturnsAsync(3200);

		return this;
	}

	public MockBaseStatsRepository SetupHighestMatches()
	{
		Setup(x => x.HighestMatchesAsync(It.IsAny<int>(), It.IsAny<string?>())).ReturnsAsync(500);

		return this;
	}

	public MockBaseStatsRepository SetupHighestRank()
	{
		Setup(x => x.HighestRankAsync(It.IsAny<int>(), It.IsAny<string?>())).ReturnsAsync(100_000_000);

		return this;
	}

	public MockBaseStatsRepository SetupGetForPlayerAsync()
	{
		Setup(x => x.GetForPlayerAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(SeededBaseStats.Get());

		return this;
	}
}