using API.DTOs;
using API.Enums;
using API.Services.Implementations;
using API.Utilities;
using APITests.MockRepositories;
using Database.Enums;

namespace APITests.Services;

public class LeaderboardServiceTests
{
    private readonly LeaderboardService _leaderboardService;

    public LeaderboardServiceTests()
    {
        MockPlayerRepository playerRepository = new MockPlayerRepository()
            .SetupGetId()
            .SetupGetCountry()
            .SetupGetOsuId()
            .SetupGetUsername()
            .SetupGetCountry();

        MockBaseStatsRepository baseStatsRepository = new MockBaseStatsRepository()
            .SetupLeaderboard()
            .SetupLeaderboardCount()
            .SetupHighestMatches()
            .SetupHighestRating()
            .SetupHighestRank()
            .SetupGetForPlayerAsync();

        MockMatchStatsRepository matchStatsRepository = new MockMatchStatsRepository()
            .SetupGlobalWinRate()
            .SetupCountMatchesPlayed();

        MockTournamentsRepository tournamentsRepository = new MockTournamentsRepository().SetupCountPlayed();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _ = new PlayerService(playerRepository.Object, null);
        _ = new PlayerStatsService(
            null,
            null,
            null,
            matchStatsRepository.Object,
            playerRepository.Object,
            null,
            null,
            null,
            null
        );

        var tournamentsService = new TournamentsService(tournamentsRepository.Object, null, null);

        var baseStatsService = new BaseStatsService(
            baseStatsRepository.Object,
            matchStatsRepository.Object,
            playerRepository.Object,
            tournamentsService
        );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        _leaderboardService = new LeaderboardService(
            playerRepository.Object,
            baseStatsService
        );
    }

    [Theory]
    [InlineData(Ruleset.Osu)]
    [InlineData(Ruleset.Taiko)]
    [InlineData(Ruleset.Catch)]
    [InlineData(Ruleset.ManiaOther)]
    [InlineData(Ruleset.Mania4k)]
    [InlineData(Ruleset.Mania7k)]
    public async Task GetLeaderboardAsync_ReturnsLeaderboard_WithCorrectMode(Ruleset ruleset)
    {
        // Arrange
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(
            new LeaderboardRequestQueryDTO { Ruleset = ruleset }
        );
        // Act

        // Assert

        Assert.Equal(ruleset, lb.Ruleset);
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsEliteGrandmaster_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO
            {
                TierFilters = new LeaderboardTierFilterDTO
                {
                    FilterEliteGrandmaster = true
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsEliteGrandmaster(x.Tier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsGrandmaster_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO
            {
                TierFilters = new LeaderboardTierFilterDTO
                {
                    FilterGrandmaster = true
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsGrandmaster(x.Tier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsMaster_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO
            {
                TierFilters = new LeaderboardTierFilterDTO
                {
                    FilterMaster = true
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsMaster(x.Tier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsDiamond_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO
            {
                TierFilters = new LeaderboardTierFilterDTO
                {
                    FilterDiamond = true
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsDiamond(x.Tier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsEmerald_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO
            {
                TierFilters = new LeaderboardTierFilterDTO
                {
                    FilterEmerald = true
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsEmerald(x.Tier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsPlatinum_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO
            {
                TierFilters = new LeaderboardTierFilterDTO
                {
                    FilterPlatinum = true
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsPlatinum(x.Tier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsGold_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO
            {
                TierFilters = new LeaderboardTierFilterDTO
                {
                    FilterGold = true
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsGold(x.Tier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsSilver_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO
            {
                TierFilters = new LeaderboardTierFilterDTO
                {
                    FilterSilver = true
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsSilver(x.Tier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsBronze_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO
            {
                TierFilters = new LeaderboardTierFilterDTO
                {
                    FilterBronze = true
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsBronze(x.Tier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsAllTiers_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
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
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.NotNull(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsEliteGrandmaster(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsGrandmaster(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsMaster(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsDiamond(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsEmerald(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsPlatinum(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsGold(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsSilver(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsBronze(x.Tier));
    }

    [Fact]
    public async Task GetLeaderboardAsync_SpecificallyIncludes_Bronze_Grandmaster_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            Filter = new LeaderboardFilterDTO
            {
                TierFilters = new LeaderboardTierFilterDTO
                {
                    FilterGrandmaster = true,
                    FilterBronze = true
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsEliteGrandmaster(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsGrandmaster(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsMaster(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsDiamond(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsEmerald(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsPlatinum(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsGold(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsSilver(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsBronze(x.Tier));
    }

    [Fact]
    public async Task GetLeaderboardAsync_SpecificallyIncludes_Silver_Diamond_EliteGrandmaster_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            Filter = new LeaderboardFilterDTO
            {
                TierFilters = new LeaderboardTierFilterDTO
                {
                    FilterEliteGrandmaster = true,
                    FilterDiamond = true,
                    FilterSilver = true,
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _leaderboardService.GetLeaderboardAsync(filter);

        // Assert
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsEliteGrandmaster(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsGrandmaster(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsMaster(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsDiamond(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsEmerald(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsPlatinum(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsGold(x.Tier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsSilver(x.Tier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsBronze(x.Tier));
    }
}
