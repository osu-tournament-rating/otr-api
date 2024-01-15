using API.DTOs;
using API.Enums;
using API.Services.Implementations;
using API.Services.Interfaces;
using API.Utilities;
using APITests.MockRepositories;

namespace APITests.Services;

public class LeaderboardServiceTests
{
	private readonly ILeaderboardService _leaderboardService;

	public LeaderboardServiceTests()
	{
		var playerRepository = new MockPlayerRepository()
		                       .SetupGetId()
		                       .SetupGetCountry()
		                       .SetupGetOsuId()
		                       .SetupGetUsername()
		                       .SetupGetCountry();

		var baseStatsRepository = new MockBaseStatsRepository()
		                          .SetupLeaderboard()
		                          .SetupLeaderboardCount()
		                          .SetupHighestMatches()
		                          .SetupHighestRating()
		                          .SetupHighestRank()
		                          .SetupGetForPlayerAsync();

		var matchStatsRepository = new MockMatchStatsRepository()
		                           .SetupGlobalWinrate()
		                           .SetupCountMatchesPlayed();

		var ratingStatsRepository = new MockRatingStatsRepository()
		                            .SetupHighestGlobalRank()
		                            .SetupHighestCountryRank()
		                            .SetupGetRankChart();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
		var playerService = new PlayerService(playerRepository.Object, null);
		var playerStatsService = new PlayerStatsService(playerRepository.Object, matchStatsRepository.Object, ratingStatsRepository.Object, null, null,
			null, null, null, null);

		var baseStatsService = new BaseStatsService(baseStatsRepository.Object, matchStatsRepository.Object, ratingStatsRepository.Object, playerRepository.Object);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

		_leaderboardService = new LeaderboardService(playerRepository.Object, baseStatsService,
			ratingStatsRepository.Object, playerService, playerStatsService);
	}

	[Fact]
	public void LeaderboardFilters_Invalid_WhenAllFiltersFalse()
	{
		// Arrange
		var filter = new LeaderboardFilterDTO
		{
			TierFilters = new LeaderboardTierFilterDTO
			{
				FilterBronze = false,
				FilterSilver = false,
				FilterGold = false,
				FilterPlatinum = false,
				FilterEmerald = false,
				FilterDiamond = false,
				FilterMaster = false,
				FilterGrandmaster = false,
				FilterEliteGrandmaster = false
			}
		};
		// Act

		// Assert
		Assert.True(filter.TierFilters.IsInvalid());
	}

	[Fact]
	public void LeaderboardFilters_Valid_WhenNull()
	{
		// Arrange
		var filter = new LeaderboardFilterDTO
		{
			TierFilters = null
		};

		// Act

		// Assert
		Assert.False(filter.TierFilters.IsInvalid());
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public async Task GetLeaderboardAsync_ReturnsLeaderboard_WithCorrectMode(int mode)
	{
		// Arrange
		var lb = await _leaderboardService.GetLeaderboardAsync(new LeaderboardRequestQueryDTO
		{
			Mode = mode
		});
		// Act

		// Assert

		Assert.Equal(mode, lb.Mode);
	}

	[Fact]
	public async Task GetLeaderboardAsync_ReturnsEliteGrandmaster_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			ChartType = LeaderboardChartType.Global,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = true,
					FilterGrandmaster = false,
					FilterMaster = false,
					FilterEmerald = false,
					FilterDiamond = false,
					FilterPlatinum = false,
					FilterGold = false,
					FilterSilver = false,
					FilterBronze = false
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.All(lb.Leaderboard, x => Assert.Equal("Elite Grandmaster", x.Tier));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ReturnsGrandmaster_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			ChartType = LeaderboardChartType.Global,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = false,
					FilterGrandmaster = true,
					FilterMaster = false,
					FilterEmerald = false,
					FilterDiamond = false,
					FilterPlatinum = false,
					FilterGold = false,
					FilterSilver = false,
					FilterBronze = false
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.All(lb.Leaderboard, x => Assert.Contains("Grandmaster", x.Tier));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ReturnsMaster_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			ChartType = LeaderboardChartType.Global,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = false,
					FilterGrandmaster = false,
					FilterMaster = true,
					FilterEmerald = false,
					FilterDiamond = false,
					FilterPlatinum = false,
					FilterGold = false,
					FilterSilver = false,
					FilterBronze = false
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.All(lb.Leaderboard, x => Assert.Contains("Master", x.Tier));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ReturnsDiamond_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			ChartType = LeaderboardChartType.Global,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = false,
					FilterGrandmaster = false,
					FilterMaster = false,
					FilterEmerald = false,
					FilterDiamond = true,
					FilterPlatinum = false,
					FilterGold = false,
					FilterSilver = false,
					FilterBronze = false
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.All(lb.Leaderboard, x => Assert.Contains("Diamond", x.Tier));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ReturnsEmerald_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			ChartType = LeaderboardChartType.Global,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = false,
					FilterGrandmaster = false,
					FilterMaster = false,
					FilterEmerald = true,
					FilterDiamond = false,
					FilterPlatinum = false,
					FilterGold = false,
					FilterSilver = false,
					FilterBronze = false
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.All(lb.Leaderboard, x => Assert.Contains("Emerald", x.Tier));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ReturnsPlatinum_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			ChartType = LeaderboardChartType.Global,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = false,
					FilterGrandmaster = false,
					FilterMaster = false,
					FilterEmerald = false,
					FilterDiamond = false,
					FilterPlatinum = true,
					FilterGold = false,
					FilterSilver = false,
					FilterBronze = false
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.All(lb.Leaderboard, x => Assert.Contains("Platinum", x.Tier));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ReturnsGold_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			ChartType = LeaderboardChartType.Global,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = false,
					FilterGrandmaster = false,
					FilterMaster = false,
					FilterEmerald = false,
					FilterDiamond = false,
					FilterPlatinum = false,
					FilterGold = true,
					FilterSilver = false,
					FilterBronze = false
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.All(lb.Leaderboard, x => Assert.Contains("Gold", x.Tier));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ReturnsSilver_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			ChartType = LeaderboardChartType.Global,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = false,
					FilterGrandmaster = false,
					FilterMaster = false,
					FilterEmerald = false,
					FilterDiamond = false,
					FilterPlatinum = false,
					FilterGold = false,
					FilterSilver = true,
					FilterBronze = false
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.All(lb.Leaderboard, x => Assert.Contains("Silver", x.Tier));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ReturnsBronze_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			ChartType = LeaderboardChartType.Global,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = false,
					FilterGrandmaster = false,
					FilterMaster = false,
					FilterEmerald = false,
					FilterDiamond = false,
					FilterPlatinum = false,
					FilterGold = false,
					FilterSilver = false,
					FilterBronze = true
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.All(lb.Leaderboard, x => Assert.Contains("Bronze", x.Tier));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ReturnsAllTiers_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = true,
					FilterGrandmaster = true,
					FilterMaster = true,
					FilterEmerald = true,
					FilterDiamond = true,
					FilterPlatinum = true,
					FilterGold = true,
					FilterSilver = true,
					FilterBronze = true
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.All(lb.Leaderboard, x => Assert.NotNull(x.Tier));
		Assert.Contains(lb.Leaderboard, x => x.Tier == "Elite Grandmaster");
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Grandmaster"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Master"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Emerald"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Diamond"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Platinum"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Gold"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Silver"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Bronze"));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ExcludesEliteGrandmaster_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = false,
					FilterGrandmaster = true,
					FilterMaster = true,
					FilterEmerald = true,
					FilterDiamond = true,
					FilterPlatinum = true,
					FilterGold = true,
					FilterSilver = true,
					FilterBronze = true
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.DoesNotContain(lb.Leaderboard, x => x.Tier == "Elite Grandmaster");
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Grandmaster"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Master"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Emerald"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Diamond"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Platinum"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Gold"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Silver"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Bronze"));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ExcludesGrandmaster_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = true,
					FilterGrandmaster = false,
					FilterMaster = true,
					FilterEmerald = true,
					FilterDiamond = true,
					FilterPlatinum = true,
					FilterGold = true,
					FilterSilver = true,
					FilterBronze = true
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert

		Assert.Contains(lb.Leaderboard, x => x.Tier == "Elite Grandmaster");
		Assert.DoesNotContain(lb.Leaderboard, x => x.Tier.Contains("Grandmaster") && x.Tier != "Elite Grandmaster");
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Master"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Emerald"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Diamond"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Platinum"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Gold"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Silver"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Bronze"));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ExcludesMaster_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = true,
					FilterGrandmaster = true,
					FilterMaster = false,
					FilterEmerald = true,
					FilterDiamond = true,
					FilterPlatinum = true,
					FilterGold = true,
					FilterSilver = true,
					FilterBronze = true
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert

		Assert.Contains(lb.Leaderboard, x => x.Tier == "Elite Grandmaster");
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Grandmaster"));
		Assert.DoesNotContain(lb.Leaderboard, x => x.Tier.Contains("Master"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Emerald"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Diamond"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Platinum"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Gold"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Silver"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Bronze"));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ExcludesEmerald_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = true,
					FilterGrandmaster = true,
					FilterMaster = true,
					FilterEmerald = false,
					FilterDiamond = true,
					FilterPlatinum = true,
					FilterGold = true,
					FilterSilver = true,
					FilterBronze = true
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		Assert.Contains(lb.Leaderboard, x => x.Tier == "Elite Grandmaster");
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Grandmaster"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Master"));
		Assert.DoesNotContain(lb.Leaderboard, x => x.Tier.Contains("Emerald"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Diamond"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Platinum"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Gold"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Silver"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Bronze"));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ExcludesDiamond_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = true,
					FilterGrandmaster = true,
					FilterMaster = true,
					FilterEmerald = true,
					FilterDiamond = false,
					FilterPlatinum = true,
					FilterGold = true,
					FilterSilver = true,
					FilterBronze = true
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.Contains(lb.Leaderboard, x => x.Tier == "Elite Grandmaster");
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Grandmaster"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Master"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Emerald"));
		Assert.DoesNotContain(lb.Leaderboard, x => x.Tier.Contains("Diamond"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Platinum"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Gold"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Silver"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Bronze"));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ExcludesPlatinum_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = true,
					FilterGrandmaster = true,
					FilterMaster = true,
					FilterEmerald = true,
					FilterDiamond = true,
					FilterPlatinum = false,
					FilterGold = true,
					FilterSilver = true,
					FilterBronze = true
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.Contains(lb.Leaderboard, x => x.Tier == "Elite Grandmaster");
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Grandmaster"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Master"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Emerald"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Diamond"));
		Assert.DoesNotContain(lb.Leaderboard, x => x.Tier.Contains("Platinum"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Gold"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Silver"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Bronze"));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ExcludesGold_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = true,
					FilterGrandmaster = true,
					FilterMaster = true,
					FilterEmerald = true,
					FilterDiamond = true,
					FilterPlatinum = true,
					FilterGold = false,
					FilterSilver = true,
					FilterBronze = true
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.Contains(lb.Leaderboard, x => x.Tier == "Elite Grandmaster");
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Grandmaster"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Master"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Emerald"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Diamond"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Platinum"));
		Assert.DoesNotContain(lb.Leaderboard, x => x.Tier.Contains("Gold"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Silver"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Bronze"));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ExcludesSilver_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = true,
					FilterGrandmaster = true,
					FilterMaster = true,
					FilterEmerald = true,
					FilterDiamond = true,
					FilterPlatinum = true,
					FilterGold = true,
					FilterSilver = false,
					FilterBronze = true
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.Contains(lb.Leaderboard, x => x.Tier == "Elite Grandmaster");
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Grandmaster"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Master"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Emerald"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Diamond"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Platinum"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Gold"));
		Assert.DoesNotContain(lb.Leaderboard, x => x.Tier.Contains("Silver"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Bronze"));
	}

	[Fact]
	public async Task GetLeaderboardAsync_ExcludesBronze_WhenRequested()
	{
		// Arrange
		var filter = new LeaderboardRequestQueryDTO
		{
			Mode = 0,
			Filter = new LeaderboardFilterDTO
			{
				TierFilters = new LeaderboardTierFilterDTO
				{
					FilterEliteGrandmaster = true,
					FilterGrandmaster = true,
					FilterMaster = true,
					FilterEmerald = true,
					FilterDiamond = true,
					FilterPlatinum = true,
					FilterGold = true,
					FilterSilver = true,
					FilterBronze = false
				}
			}
		};

		// Act 
		var lb = await _leaderboardService.GetLeaderboardAsync(filter);

		// Assert
		Assert.Contains(lb.Leaderboard, x => x.Tier == "Elite Grandmaster");
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Grandmaster"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Master"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Emerald"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Diamond"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Platinum"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Gold"));
		Assert.Contains(lb.Leaderboard, x => x.Tier.Contains("Silver"));
		Assert.DoesNotContain(lb.Leaderboard, x => x.Tier.Contains("Bronze"));
	}
}