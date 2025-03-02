using API.DTOs;
using API.Services.Implementations;
using API.Utilities;
using APITests.MockRepositories;
using AutoMapper;
using Common.Enums;
using Common.Enums.Enums;
using MapperProfile = OsuApiClient.Configurations.MapperProfile;

namespace APITests.Services;

public class LeaderboardServiceTests
{
    private readonly PlayerRatingsService _playerRatingsService;

    public LeaderboardServiceTests()
    {
        MockPlayerRepository playerRepository = new MockPlayerRepository()
            .SetupGetId()
            .SetupGetCountry()
            .SetupGetOsuId()
            .SetupGetUsername()
            .SetupGetCountry();

        MockPlayerRatingsRepository playerRatingsRepository = new MockPlayerRatingsRepository()
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

        var tournamentsService = new TournamentsService(tournamentsRepository.Object, null, null, null);

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        _playerRatingsService = new PlayerRatingsService(
            playerRatingsRepository.Object,
            matchStatsRepository.Object,
            playerRepository.Object,
            tournamentsService,
            new Mapper(new MapperConfiguration(configuration => configuration.AddProfile(new MapperProfile())))
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
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(
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
                TierFilters = new LeaderboardTierFilterDTO { FilterEliteGrandmaster = true }
            }
        };

        // Act
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsEliteGrandmaster(x.CurrentTier)));
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
                TierFilters = new LeaderboardTierFilterDTO { FilterGrandmaster = true }
            }
        };

        // Act
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsGrandmaster(x.CurrentTier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsMaster_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO { TierFilters = new LeaderboardTierFilterDTO { FilterMaster = true } }
        };

        // Act
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsMaster(x.CurrentTier)));
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
                TierFilters = new LeaderboardTierFilterDTO { FilterDiamond = true }
            }
        };

        // Act
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsDiamond(x.CurrentTier)));
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
                TierFilters = new LeaderboardTierFilterDTO { FilterEmerald = true }
            }
        };

        // Act
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsEmerald(x.CurrentTier)));
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
                TierFilters = new LeaderboardTierFilterDTO { FilterPlatinum = true }
            }
        };

        // Act
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsPlatinum(x.CurrentTier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsGold_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO { TierFilters = new LeaderboardTierFilterDTO { FilterGold = true } }
        };

        // Act
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsGold(x.CurrentTier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsSilver_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO { TierFilters = new LeaderboardTierFilterDTO { FilterSilver = true } }
        };

        // Act
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsSilver(x.CurrentTier)));
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsBronze_WhenRequested()
    {
        // Arrange
        var filter = new LeaderboardRequestQueryDTO
        {
            Ruleset = 0,
            ChartType = LeaderboardChartType.Global,
            Filter = new LeaderboardFilterDTO { TierFilters = new LeaderboardTierFilterDTO { FilterBronze = true } }
        };

        // Act
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.True(RatingUtils.IsBronze(x.CurrentTier)));
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
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.All(lb.Leaderboard, x => Assert.NotNull(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsEliteGrandmaster(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsGrandmaster(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsMaster(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsDiamond(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsEmerald(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsPlatinum(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsGold(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsSilver(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsBronze(x.CurrentTier));
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
                TierFilters = new LeaderboardTierFilterDTO { FilterGrandmaster = true, FilterBronze = true }
            }
        };

        // Act
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsEliteGrandmaster(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsGrandmaster(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsMaster(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsDiamond(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsEmerald(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsPlatinum(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsGold(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsSilver(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsBronze(x.CurrentTier));
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
                    FilterSilver = true
                }
            }
        };

        // Act
        LeaderboardDTO lb = await _playerRatingsService.GetLeaderboardAsync(filter);

        // Assert
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsEliteGrandmaster(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsGrandmaster(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsMaster(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsDiamond(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsEmerald(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsPlatinum(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsGold(x.CurrentTier));
        Assert.Contains(lb.Leaderboard, x => RatingUtils.IsSilver(x.CurrentTier));
        Assert.DoesNotContain(lb.Leaderboard, x => RatingUtils.IsBronze(x.CurrentTier));
    }
}
