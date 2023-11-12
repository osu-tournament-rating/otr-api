using API.DTOs;
using API.Enums;
using APITests.Instances;

namespace APITests.Services;

[Collection("DatabaseCollection")]
public class LeaderboardServiceTests
{
	private readonly TestDatabaseFixture _fixture;
	public LeaderboardServiceTests(TestDatabaseFixture fixture) { _fixture = fixture; }
	public static IEnumerable<object[]> InvalidQueryFilter => new List<object[]>
	{
		new object[] { new LeaderboardFilterDTO { MinRank = 0 } },
		new object[] { new LeaderboardFilterDTO { MaxRank = 0 } },
		new object[] { new LeaderboardFilterDTO { MinRank = 5, MaxRank = 4 } },
		new object[] { new LeaderboardFilterDTO { MinRating = 0 } },
		new object[] { new LeaderboardFilterDTO { MaxRating = 0 } },
		new object[] { new LeaderboardFilterDTO { MinRating = 5, MaxRating = 4 } },
		new object[] { new LeaderboardFilterDTO { MinMatches = -1 } },
		new object[] { new LeaderboardFilterDTO { MaxMatches = -1 } },
		new object[] { new LeaderboardFilterDTO { MinMatches = 5, MaxMatches = 4 } },
		new object[] { new LeaderboardFilterDTO { MinWinrate = -0.1 } },
		new object[] { new LeaderboardFilterDTO { MaxWinrate = -0.1 } },
		new object[] { new LeaderboardFilterDTO { MinWinrate = 1.1 } },
		new object[] { new LeaderboardFilterDTO { MaxWinrate = 1.1 } },
		new object[] { new LeaderboardFilterDTO { MinWinrate = 0.5, MaxWinrate = 0.4 } }
	};
	public static IEnumerable<object[]> ForceIncludeTieredQueryFilters => new List<object[]>
	{
		new object[]
		{
			new LeaderboardTierFilterDTO
			{
				FilterBronze = true, FilterSilver = false, FilterGold = false, FilterPlatinum = false, FilterDiamond = false, FilterMaster = false, FilterGrandmaster = false,
				FilterEliteGrandmaster = false
			},
			"Bronze"
		},
		new object[]
		{
			new LeaderboardTierFilterDTO
			{
				FilterBronze = false, FilterSilver = true, FilterGold = false, FilterPlatinum = false, FilterDiamond = false, FilterMaster = false, FilterGrandmaster = false,
				FilterEliteGrandmaster = false
			},
			"Silver"
		},
		new object[]
		{
			new LeaderboardTierFilterDTO
			{
				FilterBronze = false, FilterSilver = false, FilterGold = true, FilterPlatinum = false, FilterDiamond = false, FilterMaster = false, FilterGrandmaster = false,
				FilterEliteGrandmaster = false
			},
			"Gold"
		},
		new object[]
		{
			new LeaderboardTierFilterDTO
			{
				FilterBronze = false, FilterSilver = false, FilterGold = false, FilterPlatinum = true, FilterDiamond = false, FilterMaster = false, FilterGrandmaster = false,
				FilterEliteGrandmaster = false
			},
			"Platinum"
		},
		new object[]
		{
			new LeaderboardTierFilterDTO
			{
				FilterBronze = false, FilterSilver = false, FilterGold = false, FilterPlatinum = false, FilterDiamond = true, FilterMaster = false, FilterGrandmaster = false,
				FilterEliteGrandmaster = false
			},
			"Diamond"
		},
		new object[]
		{
			new LeaderboardTierFilterDTO
			{
				FilterBronze = false, FilterSilver = false, FilterGold = false, FilterPlatinum = false, FilterDiamond = false, FilterMaster = true, FilterGrandmaster = false,
				FilterEliteGrandmaster = false
			},
			"Master"
		},
		new object[]
		{
			new LeaderboardTierFilterDTO
			{
				FilterBronze = false, FilterSilver = false, FilterGold = false, FilterPlatinum = false, FilterDiamond = false, FilterMaster = false, FilterGrandmaster = true,
				FilterEliteGrandmaster = false
			},
			"Grandmaster"
		},
		new object[]
		{
			new LeaderboardTierFilterDTO
			{
				FilterBronze = false, FilterSilver = false, FilterGold = false, FilterPlatinum = false, FilterDiamond = false, FilterMaster = false, FilterGrandmaster = false,
				FilterEliteGrandmaster = true
			},
			"Elite Grandmaster"
		}
	};
	public static IEnumerable<object[]> ForceExcludeQueryFilters => new List<object[]>
	{
		new object[] { new LeaderboardTierFilterDTO { FilterBronze = false }, "Bronze" },
		new object[] { new LeaderboardTierFilterDTO { FilterSilver = false }, "Silver" },
		new object[] { new LeaderboardTierFilterDTO { FilterGold = false }, "Gold" },
		new object[] { new LeaderboardTierFilterDTO { FilterPlatinum = false }, "Platinum" },
		new object[] { new LeaderboardTierFilterDTO { FilterDiamond = false }, "Diamond" },
		new object[] { new LeaderboardTierFilterDTO { FilterMaster = false }, "Master" },
		new object[] { new LeaderboardTierFilterDTO { FilterGrandmaster = false }, "Grandmaster" },
		new object[] { new LeaderboardTierFilterDTO { FilterEliteGrandmaster = false }, "Elite Grandmaster" }
	};

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
			PlayerId = userId
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
	public async Task Leaderboard_Players_OfSameCountry()
	{
		using var context = _fixture.CreateContext();
		var service = ServiceInstances.LeaderboardService(context);

		const string country = "US";

		var query = new LeaderboardRequestQueryDTO
		{
			PlayerId = 440,
			ChartType = LeaderboardChartType.Country
		};

		var result = await service.GetLeaderboardAsync(query);

		Assert.All(result.PlayerInfo, pInfo => Assert.Equal(country, pInfo.Country));
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

	[Theory] [MemberData(nameof(ForceIncludeTieredQueryFilters))]
	public async Task Leaderboard_ContainsOnlyExplicitPlayers_WhenFilterForced(LeaderboardTierFilterDTO filter, string tier)
	{
		using var context = _fixture.CreateContext();
		var service = ServiceInstances.LeaderboardService(context);

		var query = new LeaderboardRequestQueryDTO
		{
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = filter
			}
		};

		var result = await service.GetLeaderboardAsync(query);

		Assert.All(result.PlayerInfo, pInfo => Assert.Equal(tier, pInfo.Tier));
	}

	[Theory] [MemberData(nameof(ForceExcludeQueryFilters))]
	public async Task Leaderboard_DoesNotContainPlayers_WhenTierExcluded(LeaderboardTierFilterDTO filter, string tier)
	{
		// This test is kinda crappy, but it's the best we can do for now.
		using var context = _fixture.CreateContext();
		var service = ServiceInstances.LeaderboardService(context);

		var query = new LeaderboardRequestQueryDTO
		{
			PageSize = 10,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = filter
			}
		};

		var result = await service.GetLeaderboardAsync(query);

		Assert.All(result.PlayerInfo, pInfo => Assert.NotEqual(tier, pInfo.Tier));
	}

	[Fact]
	public async Task Leaderboard_HasTotalPlayerCount_GreaterThanZero()
	{
		using var context = _fixture.CreateContext();
		var service = ServiceInstances.LeaderboardService(context);

		var query = new LeaderboardRequestQueryDTO();

		var result = await service.GetLeaderboardAsync(query);

		Assert.True(result.TotalPlayerCount > 0);
	}
}