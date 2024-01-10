using API.DTOs;
using API.Enums;
using API.Services.Implementations;
using API.Services.Interfaces;
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
}