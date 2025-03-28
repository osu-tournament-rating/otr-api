using API.Utilities;
using Common.Rating;

namespace APITests.Utilities;

public class RatingUtilsTests
{
    [Theory]
    [InlineData(RatingConstants.RatingBronzeIII, "Bronze III")]
    [InlineData(RatingConstants.RatingBronzeII, "Bronze II")]
    [InlineData(RatingConstants.RatingBronzeI, "Bronze I")]
    [InlineData(RatingConstants.RatingSilverIII, "Silver III")]
    [InlineData(RatingConstants.RatingSilverII, "Silver II")]
    [InlineData(RatingConstants.RatingSilverI, "Silver I")]
    [InlineData(RatingConstants.RatingGoldIII, "Gold III")]
    [InlineData(RatingConstants.RatingGoldII, "Gold II")]
    [InlineData(RatingConstants.RatingGoldI, "Gold I")]
    [InlineData(RatingConstants.RatingPlatinumIII, "Platinum III")]
    [InlineData(RatingConstants.RatingPlatinumII, "Platinum II")]
    [InlineData(RatingConstants.RatingPlatinumI, "Platinum I")]
    [InlineData(RatingConstants.RatingEmeraldIII, "Emerald III")]
    [InlineData(RatingConstants.RatingEmeraldII, "Emerald II")]
    [InlineData(RatingConstants.RatingEmeraldI, "Emerald I")]
    [InlineData(RatingConstants.RatingDiamondIII, "Diamond III")]
    [InlineData(RatingConstants.RatingDiamondII, "Diamond II")]
    [InlineData(RatingConstants.RatingDiamondI, "Diamond I")]
    [InlineData(RatingConstants.RatingMasterIII, "Master III")]
    [InlineData(RatingConstants.RatingMasterII, "Master II")]
    [InlineData(RatingConstants.RatingMasterI, "Master I")]
    [InlineData(RatingConstants.RatingGrandmasterIII, "Grandmaster III")]
    [InlineData(RatingConstants.RatingGrandmasterII, "Grandmaster II")]
    [InlineData(RatingConstants.RatingGrandmasterI, "Grandmaster I")]
    [InlineData(RatingConstants.RatingEliteGrandmaster, "Elite Grandmaster")]
    public void GetTier_ReturnsCorrectTier(double rating, string expectedTier)
    {
        // Arrange

        // Act
        var actualTier = RatingUtils.GetTier(rating);

        // Assert
        Assert.Equal(expectedTier, actualTier);
    }

    [Theory]
    [InlineData(RatingConstants.RatingBronzeIII, 3)]
    [InlineData(RatingConstants.RatingBronzeII, 2)]
    [InlineData(RatingConstants.RatingBronzeI, 1)]
    [InlineData(RatingConstants.RatingSilverIII, 3)]
    [InlineData(RatingConstants.RatingSilverII, 2)]
    [InlineData(RatingConstants.RatingSilverI, 1)]
    [InlineData(RatingConstants.RatingGoldIII, 3)]
    [InlineData(RatingConstants.RatingGoldII, 2)]
    [InlineData(RatingConstants.RatingGoldI, 1)]
    [InlineData(RatingConstants.RatingPlatinumIII, 3)]
    [InlineData(RatingConstants.RatingPlatinumII, 2)]
    [InlineData(RatingConstants.RatingPlatinumI, 1)]
    [InlineData(RatingConstants.RatingEmeraldIII, 3)]
    [InlineData(RatingConstants.RatingEmeraldII, 2)]
    [InlineData(RatingConstants.RatingEmeraldI, 1)]
    [InlineData(RatingConstants.RatingDiamondIII, 3)]
    [InlineData(RatingConstants.RatingDiamondII, 2)]
    [InlineData(RatingConstants.RatingDiamondI, 1)]
    [InlineData(RatingConstants.RatingMasterIII, 3)]
    [InlineData(RatingConstants.RatingMasterII, 2)]
    [InlineData(RatingConstants.RatingMasterI, 1)]
    [InlineData(RatingConstants.RatingGrandmasterIII, 3)]
    [InlineData(RatingConstants.RatingGrandmasterII, 2)]
    [InlineData(RatingConstants.RatingGrandmasterI, 1)]
    public void GetSubTier_ReturnsCorrectSubTier(double rating, int expectedSubTier)
    {
        // Arrange

        // Act
        var actualSubTier = RatingUtils.GetSubTier(rating);

        // Assert
        Assert.Equal(expectedSubTier, actualSubTier);
    }

    [Fact]
    public void GetSubTier_GivenAnyAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingConstants.RatingEliteGrandmaster + 1;

        // Act
        var subTier = RatingUtils.GetSubTier(aboveEliteGrandmaster);

        // Arrange
        Assert.Null(subTier);
    }

    [Theory]
    [InlineData(RatingConstants.RatingBronzeII)]
    [InlineData(RatingConstants.RatingBronzeI)]
    [InlineData(RatingConstants.RatingSilverIII)]
    [InlineData(RatingConstants.RatingSilverII)]
    [InlineData(RatingConstants.RatingSilverI)]
    [InlineData(RatingConstants.RatingGoldIII)]
    [InlineData(RatingConstants.RatingGoldII)]
    [InlineData(RatingConstants.RatingGoldI)]
    [InlineData(RatingConstants.RatingPlatinumIII)]
    [InlineData(RatingConstants.RatingPlatinumII)]
    [InlineData(RatingConstants.RatingPlatinumI)]
    [InlineData(RatingConstants.RatingEmeraldIII)]
    [InlineData(RatingConstants.RatingEmeraldII)]
    [InlineData(RatingConstants.RatingEmeraldI)]
    [InlineData(RatingConstants.RatingDiamondIII)]
    [InlineData(RatingConstants.RatingDiamondII)]
    [InlineData(RatingConstants.RatingDiamondI)]
    [InlineData(RatingConstants.RatingMasterIII)]
    [InlineData(RatingConstants.RatingMasterII)]
    [InlineData(RatingConstants.RatingMasterI)]
    [InlineData(RatingConstants.RatingGrandmasterIII)]
    [InlineData(RatingConstants.RatingGrandmasterII)]
    [InlineData(RatingConstants.RatingGrandmasterI)]
    [InlineData(RatingConstants.RatingEliteGrandmaster)]
    public void GetNextTier_ReturnsCorrectNextTier(double rating)
    {
        // Arrange
        var belowRating = rating - 1;
        var expectedTier = RatingUtils.GetTier(rating);

        // Act
        var actualNextTier = RatingUtils.GetNextTier(belowRating);

        // Assert
        Assert.Equal(actualNextTier, expectedTier);
    }

    [Fact]
    public void GetNextTier_GivenAnyRatingAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingConstants.RatingEliteGrandmaster + 1;

        // Act
        var nextTier = RatingUtils.GetNextTier(aboveEliteGrandmaster);

        // Assert
        Assert.Null(nextTier);
    }

    [Theory]
    [InlineData(RatingConstants.RatingBronzeII)]
    [InlineData(RatingConstants.RatingBronzeI)]
    [InlineData(RatingConstants.RatingSilverIII)]
    [InlineData(RatingConstants.RatingSilverII)]
    [InlineData(RatingConstants.RatingSilverI)]
    [InlineData(RatingConstants.RatingGoldIII)]
    [InlineData(RatingConstants.RatingGoldII)]
    [InlineData(RatingConstants.RatingGoldI)]
    [InlineData(RatingConstants.RatingPlatinumIII)]
    [InlineData(RatingConstants.RatingPlatinumII)]
    [InlineData(RatingConstants.RatingPlatinumI)]
    [InlineData(RatingConstants.RatingEmeraldIII)]
    [InlineData(RatingConstants.RatingEmeraldII)]
    [InlineData(RatingConstants.RatingEmeraldI)]
    [InlineData(RatingConstants.RatingDiamondIII)]
    [InlineData(RatingConstants.RatingDiamondII)]
    [InlineData(RatingConstants.RatingDiamondI)]
    [InlineData(RatingConstants.RatingMasterIII)]
    [InlineData(RatingConstants.RatingMasterII)]
    [InlineData(RatingConstants.RatingMasterI)]
    [InlineData(RatingConstants.RatingGrandmasterIII)]
    [InlineData(RatingConstants.RatingGrandmasterII)]
    [InlineData(RatingConstants.RatingGrandmasterI)]
    [InlineData(RatingConstants.RatingEliteGrandmaster)]
    public void GetNextTierRating_ReturnsCorrectRating(double rating)
    {
        // Arrange
        var belowRating = rating - 1;

        // Act
        var nextTierRating = RatingUtils.GetNextTierRating(belowRating);

        // Assert
        Assert.Equal(rating, nextTierRating);
    }

    [Fact]
    public void GetNextTierRating_GivenAnyAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingConstants.RatingEliteGrandmaster + 1;

        // Act
        var nextTierRating = RatingUtils.GetNextTierRating(aboveEliteGrandmaster);

        // Assert
        Assert.Null(nextTierRating);
    }

    [Theory]
    [InlineData(RatingConstants.RatingBronzeIII, 5)]
    [InlineData(RatingConstants.RatingBronzeII, 15)]
    [InlineData(RatingConstants.RatingBronzeI, 25)]
    [InlineData(RatingConstants.RatingSilverIII, 23)]
    [InlineData(RatingConstants.RatingSilverII, 45)]
    [InlineData(RatingConstants.RatingSilverI, 54)]
    [InlineData(RatingConstants.RatingGoldIII, 52)]
    [InlineData(RatingConstants.RatingGoldII, 24)]
    [InlineData(RatingConstants.RatingGoldI, 32)]
    [InlineData(RatingConstants.RatingPlatinumIII, 5)]
    [InlineData(RatingConstants.RatingPlatinumII, 5)]
    [InlineData(RatingConstants.RatingPlatinumI, 5)]
    [InlineData(RatingConstants.RatingEmeraldIII, 5)]
    [InlineData(RatingConstants.RatingEmeraldII, 5)]
    [InlineData(RatingConstants.RatingEmeraldI, 5)]
    [InlineData(RatingConstants.RatingDiamondIII, 5)]
    [InlineData(RatingConstants.RatingDiamondII, 5)]
    [InlineData(RatingConstants.RatingDiamondI, 5)]
    [InlineData(RatingConstants.RatingMasterIII, 5)]
    [InlineData(RatingConstants.RatingMasterII, 5)]
    [InlineData(RatingConstants.RatingMasterI, 5)]
    [InlineData(RatingConstants.RatingGrandmasterIII, 5)]
    [InlineData(RatingConstants.RatingGrandmasterII, 5)]
    [InlineData(RatingConstants.RatingGrandmasterI, 5)]
    public void GetNextTierRatingDelta_ReturnsCorrectDelta(double rating, double offset)
    {
        // Arrange
        var ratingNextTier = RatingUtils.GetNextTierRating(rating);
        var expectedDelta = ratingNextTier - rating - offset;

        // Act
        var actualDelta = RatingUtils.GetNextTierRatingDelta(rating + offset);

        // Assert
        Assert.Equal(expectedDelta, actualDelta);
    }

    [Fact]
    public void GetNextTierRatingDelta_GivenAnyAboveEliteGrandmaster_ReturnsZero()
    {
        // Arrange
        const double ratingAboveEliteGrandmaster = RatingConstants.RatingEliteGrandmaster + 1;
        const int expectedDelta = 0;

        // Act
        var nextTierRatingDelta = RatingUtils.GetNextTierRatingDelta(ratingAboveEliteGrandmaster);

        // Assert
        Assert.Equal(expectedDelta, nextTierRatingDelta);
    }

    [Theory]
    [InlineData(RatingConstants.RatingBronzeIII)]
    [InlineData(RatingConstants.RatingBronzeII)]
    [InlineData(RatingConstants.RatingBronzeI)]
    [InlineData(RatingConstants.RatingSilverIII)]
    [InlineData(RatingConstants.RatingSilverII)]
    [InlineData(RatingConstants.RatingSilverI)]
    [InlineData(RatingConstants.RatingGoldIII)]
    [InlineData(RatingConstants.RatingGoldII)]
    [InlineData(RatingConstants.RatingGoldI)]
    [InlineData(RatingConstants.RatingPlatinumIII)]
    [InlineData(RatingConstants.RatingPlatinumII)]
    [InlineData(RatingConstants.RatingPlatinumI)]
    [InlineData(RatingConstants.RatingEmeraldIII)]
    [InlineData(RatingConstants.RatingEmeraldII)]
    [InlineData(RatingConstants.RatingEmeraldI)]
    [InlineData(RatingConstants.RatingDiamondIII)]
    [InlineData(RatingConstants.RatingDiamondII)]
    [InlineData(RatingConstants.RatingDiamondI)]
    [InlineData(RatingConstants.RatingMasterIII)]
    [InlineData(RatingConstants.RatingMasterII)]
    [InlineData(RatingConstants.RatingMasterI)]
    [InlineData(RatingConstants.RatingGrandmasterIII)]
    [InlineData(RatingConstants.RatingGrandmasterII)]
    [InlineData(RatingConstants.RatingGrandmasterI)]
    [InlineData(RatingConstants.RatingEliteGrandmaster)]
    public void GetPreviousTierRating_ReturnsCorrectRating(double rating)
    {
        // Arrange
        var aboveRating = rating + 1;

        // Act
        var prevTierRating = RatingUtils.GetPreviousTierRating(aboveRating);

        // Assert
        Assert.Equal(rating, prevTierRating);
    }

    [Theory]
    [InlineData(RatingConstants.RatingSilverIII)]
    [InlineData(RatingConstants.RatingGoldIII)]
    [InlineData(RatingConstants.RatingPlatinumIII)]
    [InlineData(RatingConstants.RatingEmeraldIII)]
    [InlineData(RatingConstants.RatingDiamondIII)]
    [InlineData(RatingConstants.RatingMasterIII)]
    [InlineData(RatingConstants.RatingGrandmasterIII)]
    [InlineData(RatingConstants.RatingEliteGrandmaster)]
    public void GetNextMajorTier_ReturnsCorrectTier(double rating)
    {
        // Arrange
        var belowRating = rating - 1;
        var expectedMajorTier = RatingUtils.GetTier(rating);

        // Act
        var nextMajorTier = RatingUtils.GetNextMajorTier(belowRating);

        // Assert
        Assert.Equal(expectedMajorTier, nextMajorTier);
    }

    [Fact]
    public void GetNextMajorTier_GivenAnyRatingAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double ratingAboveEliteGrandmaster = RatingConstants.RatingEliteGrandmaster + 1;

        // Act
        var nextMajorTier = RatingUtils.GetNextMajorTier(ratingAboveEliteGrandmaster);

        // Assert
        Assert.Null(nextMajorTier);
    }

    [Theory]
    [InlineData(RatingConstants.RatingSilverIII)]
    [InlineData(RatingConstants.RatingGoldIII)]
    [InlineData(RatingConstants.RatingPlatinumIII)]
    [InlineData(RatingConstants.RatingEmeraldIII)]
    [InlineData(RatingConstants.RatingDiamondIII)]
    [InlineData(RatingConstants.RatingMasterIII)]
    [InlineData(RatingConstants.RatingGrandmasterIII)]
    [InlineData(RatingConstants.RatingEliteGrandmaster)]
    public void GetNextMajorTierRating_ReturnsCorrectRating(double rating)
    {
        // Arrange
        var belowRating = rating - 1;

        // Act
        var nextMajorTierRating = RatingUtils.GetNextMajorTierRating(belowRating);

        // Assert
        Assert.Equal(rating, nextMajorTierRating);
    }

    [Fact]
    public void GetNextMajorTierRating_GivenAnyAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingConstants.RatingEliteGrandmaster + 1;

        // Act
        var nextMajorTier = RatingUtils.GetNextMajorTierRating(aboveEliteGrandmaster);

        // Assert
        Assert.Null(nextMajorTier);
    }

    [Theory]
    [InlineData(RatingConstants.RatingBronzeIII, 5)]
    [InlineData(RatingConstants.RatingBronzeII, 15)]
    [InlineData(RatingConstants.RatingBronzeI, 25)]
    [InlineData(RatingConstants.RatingSilverIII, 23)]
    [InlineData(RatingConstants.RatingSilverII, 45)]
    [InlineData(RatingConstants.RatingSilverI, 54)]
    [InlineData(RatingConstants.RatingGoldIII, 52)]
    [InlineData(RatingConstants.RatingGoldII, 24)]
    [InlineData(RatingConstants.RatingGoldI, 32)]
    [InlineData(RatingConstants.RatingPlatinumIII, 5)]
    [InlineData(RatingConstants.RatingPlatinumII, 5)]
    [InlineData(RatingConstants.RatingPlatinumI, 5)]
    [InlineData(RatingConstants.RatingEmeraldIII, 5)]
    [InlineData(RatingConstants.RatingEmeraldII, 5)]
    [InlineData(RatingConstants.RatingEmeraldI, 5)]
    [InlineData(RatingConstants.RatingDiamondIII, 5)]
    [InlineData(RatingConstants.RatingDiamondII, 5)]
    [InlineData(RatingConstants.RatingDiamondI, 5)]
    [InlineData(RatingConstants.RatingMasterIII, 5)]
    [InlineData(RatingConstants.RatingMasterII, 5)]
    [InlineData(RatingConstants.RatingMasterI, 5)]
    [InlineData(RatingConstants.RatingGrandmasterIII, 5)]
    [InlineData(RatingConstants.RatingGrandmasterII, 5)]
    [InlineData(RatingConstants.RatingGrandmasterI, 5)]
    public void GetNextMajorTierRatingDelta_ReturnsCorrectDelta(double rating, double offset)
    {
        // Arrange
        var ratingNextMajorTier = RatingUtils.GetNextMajorTierRating(rating);
        var expectedDelta = ratingNextMajorTier - rating - offset;

        // Act
        var actualDelta = RatingUtils.GetNextMajorTierRatingDelta(rating + offset);

        // Assert
        Assert.Equal(expectedDelta, actualDelta);
    }

    [Fact]
    public void GetNextMajorTierRatingDelta_GivenAnyAboveEliteGrandmaster_ReturnsZero()
    {
        // Arrange
        const double ratingAboveEliteGrandmaster = RatingConstants.RatingEliteGrandmaster + 1;
        const int expectedDelta = 0;

        // Act
        var nextTierRatingDelta = RatingUtils.GetNextTierRatingDelta(ratingAboveEliteGrandmaster);

        // Assert
        Assert.Equal(expectedDelta, nextTierRatingDelta);
    }

    [Theory]
    [InlineData(RatingConstants.RatingBronzeIII)]
    [InlineData(RatingConstants.RatingSilverIII)]
    [InlineData(RatingConstants.RatingGoldIII)]
    [InlineData(RatingConstants.RatingPlatinumIII)]
    [InlineData(RatingConstants.RatingEmeraldIII)]
    [InlineData(RatingConstants.RatingDiamondIII)]
    [InlineData(RatingConstants.RatingMasterIII)]
    [InlineData(RatingConstants.RatingGrandmasterIII)]
    [InlineData(RatingConstants.RatingEliteGrandmaster)]
    public void GetMajorTierRating_ReturnsCorrectRating(double rating)
    {
        // Arrange
        var aboveRating = rating + 1;

        // Act
        var majorTierRating = RatingUtils.GetMajorTierRating(aboveRating);

        // Assert
        Assert.Equal(rating, majorTierRating);
    }

    [Theory]
    [InlineData(RatingConstants.RatingBronzeIII, 5)]
    [InlineData(RatingConstants.RatingBronzeII, 15)]
    [InlineData(RatingConstants.RatingBronzeI, 25)]
    [InlineData(RatingConstants.RatingSilverIII, 23)]
    [InlineData(RatingConstants.RatingSilverII, 45)]
    [InlineData(RatingConstants.RatingSilverI, 54)]
    [InlineData(RatingConstants.RatingGoldIII, 52)]
    [InlineData(RatingConstants.RatingGoldII, 24)]
    [InlineData(RatingConstants.RatingGoldI, 32)]
    [InlineData(RatingConstants.RatingPlatinumIII, 31)]
    [InlineData(RatingConstants.RatingPlatinumII, 29)]
    [InlineData(RatingConstants.RatingPlatinumI, 22)]
    [InlineData(RatingConstants.RatingEmeraldIII, 14)]
    [InlineData(RatingConstants.RatingEmeraldII, 19)]
    [InlineData(RatingConstants.RatingEmeraldI, 20)]
    [InlineData(RatingConstants.RatingDiamondIII, 13)]
    [InlineData(RatingConstants.RatingDiamondII, 17)]
    [InlineData(RatingConstants.RatingDiamondI, 29)]
    [InlineData(RatingConstants.RatingMasterIII, 26)]
    [InlineData(RatingConstants.RatingMasterII, 15)]
    [InlineData(RatingConstants.RatingMasterI, 5)]
    [InlineData(RatingConstants.RatingGrandmasterIII, 10)]
    [InlineData(RatingConstants.RatingGrandmasterII, 8)]
    [InlineData(RatingConstants.RatingGrandmasterI, 16)]
    public void GetNextTierFillPercentage_ReturnsCorrectPercentage(double rating, double offset)
    {
        // Arrange
        var nextTierRating = RatingUtils.GetNextTierRating(rating);
        var expectedPercentage = offset / (nextTierRating - rating);

        // Act
        var actualPercentage = RatingUtils.GetNextTierFillPercentage(rating + offset);

        // Assert
        Assert.Equal(expectedPercentage, actualPercentage);
    }

    [Fact]
    public void GetNextTierFillPercentage_GivenAnyAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingConstants.RatingEliteGrandmaster + 1;

        // Act
        var percentage = RatingUtils.GetNextTierFillPercentage(aboveEliteGrandmaster);

        // Assert
        Assert.Null(percentage);
    }

    [Theory]
    [InlineData(RatingConstants.RatingBronzeIII, 5)]
    [InlineData(RatingConstants.RatingSilverIII, 23)]
    [InlineData(RatingConstants.RatingGoldIII, 52)]
    [InlineData(RatingConstants.RatingPlatinumIII, 31)]
    [InlineData(RatingConstants.RatingEmeraldIII, 14)]
    [InlineData(RatingConstants.RatingDiamondIII, 13)]
    [InlineData(RatingConstants.RatingMasterIII, 26)]
    [InlineData(RatingConstants.RatingGrandmasterIII, 10)]
    public void GetNextMajorTierFillPercentage_ReturnsCorrectPercentage(double rating, double offset)
    {
        // Arrange
        var nextMajorTierRating = RatingUtils.GetNextMajorTierRating(rating);
        var expectedPercentage = offset / (nextMajorTierRating - rating);

        // Act
        var actualPercentage = RatingUtils.GetNextMajorTierFillPercentage(rating + offset);

        // Assert
        Assert.Equal(expectedPercentage, actualPercentage);
    }

    [Fact]
    public void GetNextMajorTierFillPercentage_GivenAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingConstants.RatingEliteGrandmaster + 1;

        // Act
        var aboveEliteGrandmasterPercentage = RatingUtils.GetNextMajorTierFillPercentage(aboveEliteGrandmaster);

        // Assert
        Assert.Null(aboveEliteGrandmasterPercentage);
    }

    [Theory]
    [InlineData(200.0, 20, 1)]
    [InlineData(200.0, 20, 3)]
    [InlineData(201, 20, 4)]
    [InlineData(50, 5, 2)]
    [InlineData(82, 20, 2)]
    [InlineData(82, 20, 1)]
    public void IsProvisional_GivenAnyCriteriaMet_ReturnsTrue(
        double volatility,
        int matchesPlayed,
        int tournamentsPlayed
    )
    {
        // Arrange

        // Act
        var isProvisional = RatingUtils.IsProvisional(volatility, matchesPlayed, tournamentsPlayed);

        // Assert
        Assert.True(isProvisional);
    }

    [Theory]
    [InlineData(199.9, 10, 3)]
    [InlineData(50, 20, 4)]
    [InlineData(21, 40, 3)]
    public void IsProvisional_GivenNotMetCriteria_ReturnsFalse(
        double volatility,
        int matchesPlayed,
        int tournamentsPlayed
    )
    {
        // Arrange

        // Act
        var isProvisional = RatingUtils.IsProvisional(volatility, matchesPlayed, tournamentsPlayed);

        // Assert
        Assert.False(isProvisional);
    }

    [Theory]
    [InlineData("Bronze III", true)]
    [InlineData("Bronze II", true)]
    [InlineData("Bronze I", true)]
    [InlineData("Garbage", false)]
    [InlineData("Silver III", false)]
    public void IsBronze_ReturnsCorrectBool(string tier, bool expected)
    {
        // Arrange

        // Act
        var actual = RatingUtils.IsBronze(tier);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Silver III", true)]
    [InlineData("Silver II", true)]
    [InlineData("Silver I", true)]
    [InlineData("Garbage", false)]
    [InlineData("Gold III", false)]
    public void IsSilver_ReturnsCorrectBool(string tier, bool expected)
    {
        // Arrange

        // Act
        var actual = RatingUtils.IsSilver(tier);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Gold III", true)]
    [InlineData("Gold II", true)]
    [InlineData("Gold I", true)]
    [InlineData("Garbage", false)]
    [InlineData("Platinum III", false)]
    public void IsGold_ReturnsCorrectBool(string tier, bool expected)
    {
        // Arrange

        // Act
        var actual = RatingUtils.IsGold(tier);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Platinum III", true)]
    [InlineData("Platinum II", true)]
    [InlineData("Platinum I", true)]
    [InlineData("Garbage", false)]
    [InlineData("Emerald III", false)]
    public void IsPlatinum_ReturnsCorrectBool(string tier, bool expected)
    {
        // Arrange

        // Act
        var actual = RatingUtils.IsPlatinum(tier);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Emerald III", true)]
    [InlineData("Emerald II", true)]
    [InlineData("Emerald I", true)]
    [InlineData("Garbage", false)]
    [InlineData("Diamond III", false)]
    public void IsEmerald_ReturnsCorrectBool(string tier, bool expected)
    {
        // Arrange

        // Act
        var actual = RatingUtils.IsEmerald(tier);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Diamond III", true)]
    [InlineData("Diamond II", true)]
    [InlineData("Diamond I", true)]
    [InlineData("Garbage", false)]
    [InlineData("Master III", false)]
    public void IsDiamond_ReturnsCorrectBool(string tier, bool expected)
    {
        // Arrange

        // Act
        var actual = RatingUtils.IsDiamond(tier);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Master III", true)]
    [InlineData("Master II", true)]
    [InlineData("Master I", true)]
    [InlineData("Garbage", false)]
    [InlineData("Grandmaster III", false)]
    public void IsMaster_ReturnsCorrectBool(string tier, bool expected)
    {
        // Arrange

        // Act
        var actual = RatingUtils.IsMaster(tier);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Grandmaster III", true)]
    [InlineData("Grandmaster II", true)]
    [InlineData("Grandmaster I", true)]
    [InlineData("Garbage", false)]
    [InlineData("Elite Grandmaster", false)]
    public void IsGrandmaster_ReturnsCorrectBool(string tier, bool expected)
    {
        // Arrange

        // Act
        var actual = RatingUtils.IsGrandmaster(tier);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Elite Grandmaster", true)]
    [InlineData("Garbage", false)]
    [InlineData("Grandmaster III", false)]
    public void IsEliteGrandmaster_ReturnsCorrectBool(string tier, bool expected)
    {
        // Arrange

        // Act
        var actual = RatingUtils.IsEliteGrandmaster(tier);

        // Assert
        Assert.Equal(expected, actual);
    }
}
