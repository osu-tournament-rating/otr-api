using API.Utilities;
using Common.Rating;

namespace APITests.Utilities;

public class RatingUtilsTests
{
    [Theory]
    [InlineData(RatingConstants.RatingBronzeIII, "Bronze")]
    [InlineData(RatingConstants.RatingBronzeII, "Bronze")]
    [InlineData(RatingConstants.RatingBronzeI, "Bronze")]
    [InlineData(RatingConstants.RatingSilverIII, "Silver")]
    [InlineData(RatingConstants.RatingSilverII, "Silver")]
    [InlineData(RatingConstants.RatingSilverI, "Silver")]
    [InlineData(RatingConstants.RatingGoldIII, "Gold")]
    [InlineData(RatingConstants.RatingGoldII, "Gold")]
    [InlineData(RatingConstants.RatingGoldI, "Gold")]
    [InlineData(RatingConstants.RatingPlatinumIII, "Platinum")]
    [InlineData(RatingConstants.RatingPlatinumII, "Platinum")]
    [InlineData(RatingConstants.RatingPlatinumI, "Platinum")]
    [InlineData(RatingConstants.RatingEmeraldIII, "Emerald")]
    [InlineData(RatingConstants.RatingEmeraldII, "Emerald")]
    [InlineData(RatingConstants.RatingEmeraldI, "Emerald")]
    [InlineData(RatingConstants.RatingDiamondIII, "Diamond")]
    [InlineData(RatingConstants.RatingDiamondII, "Diamond")]
    [InlineData(RatingConstants.RatingDiamondI, "Diamond")]
    [InlineData(RatingConstants.RatingMasterIII, "Master")]
    [InlineData(RatingConstants.RatingMasterII, "Master")]
    [InlineData(RatingConstants.RatingMasterI, "Master")]
    [InlineData(RatingConstants.RatingGrandmasterIII, "Grandmaster")]
    [InlineData(RatingConstants.RatingGrandmasterII, "Grandmaster")]
    [InlineData(RatingConstants.RatingGrandmasterI, "Grandmaster")]
    [InlineData(RatingConstants.RatingEliteGrandmaster, "Elite Grandmaster")]
    public void GetTier_ReturnsCorrectTier(double rating, string expectedTier)
    {
        // Arrange

        // Act
        var actualTier = RatingUtils.GetMajorTier(rating);

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
    [InlineData(RatingConstants.RatingBronzeII, 1)]
    [InlineData(RatingConstants.RatingBronzeI, 3)]
    [InlineData(RatingConstants.RatingSilverIII, 2)]
    [InlineData(RatingConstants.RatingSilverII, 1)]
    [InlineData(RatingConstants.RatingSilverI, 3)]
    [InlineData(RatingConstants.RatingGoldIII, 2)]
    [InlineData(RatingConstants.RatingGoldII, 1)]
    [InlineData(RatingConstants.RatingGoldI, 3)]
    [InlineData(RatingConstants.RatingPlatinumIII, 2)]
    [InlineData(RatingConstants.RatingPlatinumII, 1)]
    [InlineData(RatingConstants.RatingPlatinumI, 3)]
    [InlineData(RatingConstants.RatingEmeraldIII, 2)]
    [InlineData(RatingConstants.RatingEmeraldII, 1)]
    [InlineData(RatingConstants.RatingEmeraldI, 3)]
    [InlineData(RatingConstants.RatingDiamondIII, 2)]
    [InlineData(RatingConstants.RatingDiamondII, 1)]
    [InlineData(RatingConstants.RatingDiamondI, 3)]
    [InlineData(RatingConstants.RatingMasterIII, 2)]
    [InlineData(RatingConstants.RatingMasterII, 1)]
    [InlineData(RatingConstants.RatingMasterI, 3)]
    [InlineData(RatingConstants.RatingGrandmasterIII, 2)]
    [InlineData(RatingConstants.RatingGrandmasterII, 1)]
    [InlineData(RatingConstants.RatingGrandmasterI, null)]
    [InlineData(RatingConstants.RatingEliteGrandmaster, null)]
    public void GetNextSubTier_ReturnsCorrectNextSubTier(double rating, int? expectedSubTier)
    {
        // Act
        var actualNextSubTier = RatingUtils.GetNextSubTier(rating);

        // Assert
        Assert.Equal(expectedSubTier, actualNextSubTier);
    }

    [Fact]
    public void GetNextTier_GivenAnyRatingAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingConstants.RatingEliteGrandmaster + 1;

        // Act
        var nextTier = RatingUtils.GetNextMajorTier(aboveEliteGrandmaster);

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
        var expectedMajorTier = RatingUtils.GetMajorTier(rating);

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
}
