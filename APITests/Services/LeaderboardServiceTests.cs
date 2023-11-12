using API.DTOs;
using APITests.Instances;

namespace APITests.Services;

[Collection("DatabaseCollection")]
public class LeaderboardServiceTests
{
	private readonly TestDatabaseFixture _fixture;

	public LeaderboardServiceTests(TestDatabaseFixture fixture) { _fixture = fixture; }

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public async Task Leaderboard_ReturnsCorrectMode(int mode)
	{
		using var context = _fixture.CreateContext();
		var service = ServiceInstances.LeaderboardService(context);

		var query = new LeaderboardRequestQueryDTO
		{
			Mode = mode,
			PageSize = 10
		};

		var result = await service.GetLeaderboardAsync(query);

		Assert.NotNull(result);
		Assert.All(result.PlayerInfo, pInfo => Assert.Equal(mode, pInfo.Mode));
	}

	[Theory]
	[InlineData(1)]
	[InlineData(5)]
	[InlineData(10)]
	public async Task Leaderboard_PageSize_ReturnsExpectedValue(int pageSize)
	{
		using var context = _fixture.CreateContext();
		var service = ServiceInstances.LeaderboardService(context);

		var query = new LeaderboardRequestQueryDTO
		{
			PageSize = pageSize
		};

		var result = await service.GetLeaderboardAsync(query);

		Assert.Equal(pageSize, result.PlayerInfo.Count());
	}

	[Fact]
	public async Task Leaderboard_PageSize_DefaultsTo50()
	{
		using var context = _fixture.CreateContext();
		var service = ServiceInstances.LeaderboardService(context);

		var query = new LeaderboardRequestQueryDTO();

		var result = await service.GetLeaderboardAsync(query);

		Assert.Equal(50, result.PlayerInfo.Count());
	}

	[Fact]
	public async Task Leaderboard_Returns_CorrectUserData()
	{
		using var context = _fixture.CreateContext();
		var service = ServiceInstances.LeaderboardService(context);

		const int userId = 440;

		var query = new LeaderboardRequestQueryDTO
		{
			UserId = userId
		};

		var result = await service.GetLeaderboardAsync(query);
		var chart = result.PlayerChart;

		Assert.NotNull(chart);
		Assert.IsType<int>(chart.Rank);
		Assert.IsType<double>(chart.Rating);
		Assert.IsType<int>(chart.Matches);
		Assert.IsType<double>(chart.Winrate);
		Assert.IsType<double>(chart.Percentile);
		Assert.IsType<int>(chart.HighestRank);
		Assert.IsType<string>(chart.Tier);
		
		Assert.True(chart.Rank > 0);
		Assert.True(chart.Rating > 0);
		Assert.True(chart.Matches > 0);
		Assert.True(chart.Winrate >= 0);
		Assert.True(chart.Winrate <= 1);
		Assert.True(chart.Percentile >= 0);
		Assert.True(chart.Percentile <= 1);
		Assert.True(chart.HighestRank > 0);
		Assert.NotEmpty(chart.Tier);
		
		Assert.All(chart.RankChart.ChartData, x => Assert.Multiple(() =>
		{
			Assert.IsType<int>(x.Rank);
			Assert.IsType<int>(x.RankChange);
			Assert.IsType<string>(x.MatchName);
			Assert.IsType<string>(x.TournamentName);
			
			Assert.True(x.Rank > 0);
			Assert.NotEmpty(x.MatchName);
			Assert.NotEmpty(x.TournamentName);
		}));
	}

	[Theory]
	[InlineData(1, 5)]
	[InlineData(100, 500)]
	[InlineData(2500, 2501)]
	public async Task Leaderboard_FiltersRankCorrectly(int minRank, int maxRank)
	{
		using var context = _fixture.CreateContext();
		var service = ServiceInstances.LeaderboardService(context);

		var query = new LeaderboardRequestQueryDTO
		{
			Filter = new LeaderboardFilterDTO
			{
				MinRank = minRank,
				MaxRank = maxRank
			}
		};

		var result = await service.GetLeaderboardAsync(query);

		Assert.All(result.PlayerInfo, pInfo => Assert.True(pInfo.GlobalRank >= minRank && pInfo.GlobalRank <= maxRank));
	}

	[Fact]
	public async Task Leaderboard_ThrowsException_WhenInvalidRankFilter()
	{
		const int minRank = 100;
		const int maxRank = 1;

		using var context = _fixture.CreateContext();
		var service = ServiceInstances.LeaderboardService(context);

		var query = new LeaderboardRequestQueryDTO
		{
			Filter = new LeaderboardFilterDTO
			{
				MinRank = minRank,
				MaxRank = maxRank
			}
		};

		await Assert.ThrowsAsync<ArgumentException>(() => service.GetLeaderboardAsync(query));
	}

	[Theory] [MemberData(nameof(InvalidQueryFilter))]
	public async Task Leaderboard_ThrowsException_WhenInvalidFilter(LeaderboardFilterDTO filter)
	{
		using var context = _fixture.CreateContext();
		var service = ServiceInstances.LeaderboardService(context);

		var query = new LeaderboardRequestQueryDTO
		{
			Filter = filter
		};

		await Assert.ThrowsAsync<ArgumentException>(() => service.GetLeaderboardAsync(query));
	}

	public static IEnumerable<object[]> InvalidQueryFilter => new List<object[]>
	{
		new object[] { new LeaderboardFilterDTO { MinRank = 0 } },
		new object[] { new LeaderboardFilterDTO { MaxRank = 0 } },
		new object[] { new LeaderboardFilterDTO { MinRank = 5, MaxRank = 4 } },
		new object[] { new LeaderboardFilterDTO { MinRating = 0 } },
		new object[] { new LeaderboardFilterDTO { MaxRating = 0 } },
		new object[] { new LeaderboardFilterDTO { MinRating = 5, MaxRating = 4 } },
		new object[] { new LeaderboardFilterDTO { MinMatches = 0 } },
		new object[] { new LeaderboardFilterDTO { MaxMatches = 0 } },
		new object[] { new LeaderboardFilterDTO { MinMatches = 5, MaxMatches = 4 } },
		new object[] { new LeaderboardFilterDTO { MinWinrate = 0 } },
		new object[] { new LeaderboardFilterDTO { MaxWinrate = 0 } },
		new object[] { new LeaderboardFilterDTO { MinWinrate = 1.1 } },
		new object[] { new LeaderboardFilterDTO { MaxWinrate = 1.1 } },
		new object[] { new LeaderboardFilterDTO { MinWinrate = 0.5, MaxWinrate = 0.4 } }
	};
}