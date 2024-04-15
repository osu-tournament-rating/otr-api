using API.Utilities;

namespace APITests.Utilities;

public class RatingUtilsTests
{
    [Theory]
    [InlineData(RatingUtils.RatingBronzeIII, "Bronze III")]
    [InlineData(RatingUtils.RatingBronzeII, "Bronze II")]
    [InlineData(RatingUtils.RatingBronzeI, "Bronze I")]
    [InlineData(RatingUtils.RatingSilverIII, "Silver III")]
    [InlineData(RatingUtils.RatingSilverII, "Silver II")]
    [InlineData(RatingUtils.RatingSilverI, "Silver I")]
    [InlineData(RatingUtils.RatingGoldIII, "Gold III")]
    [InlineData(RatingUtils.RatingGoldII, "Gold II")]
    [InlineData(RatingUtils.RatingGoldI, "Gold I")]
    [InlineData(RatingUtils.RatingPlatinumIII, "Platinum III")]
    [InlineData(RatingUtils.RatingPlatinumII, "Platinum II")]
    [InlineData(RatingUtils.RatingPlatinumI, "Platinum I")]
    [InlineData(RatingUtils.RatingEmeraldIII, "Emerald III")]
    [InlineData(RatingUtils.RatingEmeraldII, "Emerald II")]
    [InlineData(RatingUtils.RatingEmeraldI, "Emerald I")]
    [InlineData(RatingUtils.RatingDiamondIII, "Diamond III")]
    [InlineData(RatingUtils.RatingDiamondII, "Diamond II")]
    [InlineData(RatingUtils.RatingDiamondI, "Diamond I")]
    [InlineData(RatingUtils.RatingMasterIII, "Master III")]
    [InlineData(RatingUtils.RatingMasterII, "Master II")]
    [InlineData(RatingUtils.RatingMasterI, "Master I")]
    [InlineData(RatingUtils.RatingGrandmasterIII, "Grandmaster III")]
    [InlineData(RatingUtils.RatingGrandmasterII, "Grandmaster II")]
    [InlineData(RatingUtils.RatingGrandmasterI, "Grandmaster I")]
    [InlineData(RatingUtils.RatingEliteGrandmaster, "Elite Grandmaster")]
    public void GetTier_ReturnsCorrectTier(double rating, string expectedTier)
    {
        // Arrange

        // Act
        var actualTier = RatingUtils.GetTier(rating);

        // Assert
        Assert.Equal(expectedTier, actualTier);
    }

    [Theory]
    [InlineData(RatingUtils.RatingBronzeIII, 3)]
    [InlineData(RatingUtils.RatingBronzeII, 2)]
    [InlineData(RatingUtils.RatingBronzeI, 1)]
    [InlineData(RatingUtils.RatingSilverIII, 3)]
    [InlineData(RatingUtils.RatingSilverII, 2)]
    [InlineData(RatingUtils.RatingSilverI, 1)]
    [InlineData(RatingUtils.RatingGoldIII, 3)]
    [InlineData(RatingUtils.RatingGoldII, 2)]
    [InlineData(RatingUtils.RatingGoldI, 1)]
    [InlineData(RatingUtils.RatingPlatinumIII, 3)]
    [InlineData(RatingUtils.RatingPlatinumII, 2)]
    [InlineData(RatingUtils.RatingPlatinumI, 1)]
    [InlineData(RatingUtils.RatingEmeraldIII, 3)]
    [InlineData(RatingUtils.RatingEmeraldII, 2)]
    [InlineData(RatingUtils.RatingEmeraldI, 1)]
    [InlineData(RatingUtils.RatingDiamondIII, 3)]
    [InlineData(RatingUtils.RatingDiamondII, 2)]
    [InlineData(RatingUtils.RatingDiamondI, 1)]
    [InlineData(RatingUtils.RatingMasterIII, 3)]
    [InlineData(RatingUtils.RatingMasterII, 2)]
    [InlineData(RatingUtils.RatingMasterI, 1)]
    [InlineData(RatingUtils.RatingGrandmasterIII, 3)]
    [InlineData(RatingUtils.RatingGrandmasterII, 2)]
    [InlineData(RatingUtils.RatingGrandmasterI, 1)]
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
        const double aboveEliteGrandmaster = RatingUtils.RatingEliteGrandmaster + 1;

        // Act
        var subTier = RatingUtils.GetSubTier(aboveEliteGrandmaster);

        // Arrange
        Assert.Null(subTier);
    }

    [Theory]
    [InlineData(RatingUtils.RatingBronzeII)]
    [InlineData(RatingUtils.RatingBronzeI)]
    [InlineData(RatingUtils.RatingSilverIII)]
    [InlineData(RatingUtils.RatingSilverII)]
    [InlineData(RatingUtils.RatingSilverI)]
    [InlineData(RatingUtils.RatingGoldIII)]
    [InlineData(RatingUtils.RatingGoldII)]
    [InlineData(RatingUtils.RatingGoldI)]
    [InlineData(RatingUtils.RatingPlatinumIII)]
    [InlineData(RatingUtils.RatingPlatinumII)]
    [InlineData(RatingUtils.RatingPlatinumI)]
    [InlineData(RatingUtils.RatingEmeraldIII)]
    [InlineData(RatingUtils.RatingEmeraldII)]
    [InlineData(RatingUtils.RatingEmeraldI)]
    [InlineData(RatingUtils.RatingDiamondIII)]
    [InlineData(RatingUtils.RatingDiamondII)]
    [InlineData(RatingUtils.RatingDiamondI)]
    [InlineData(RatingUtils.RatingMasterIII)]
    [InlineData(RatingUtils.RatingMasterII)]
    [InlineData(RatingUtils.RatingMasterI)]
    [InlineData(RatingUtils.RatingGrandmasterIII)]
    [InlineData(RatingUtils.RatingGrandmasterII)]
    [InlineData(RatingUtils.RatingGrandmasterI)]
    [InlineData(RatingUtils.RatingEliteGrandmaster)]
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
    public void GetNextTier_GivenAnyRatingBelowBronzeIII_ReturnsTierBronzeII()
    {
        // Arrange
        const double belowBronzeIII = RatingUtils.RatingBronzeIII + 1;
        var expectedTier = RatingUtils.GetTier(RatingUtils.RatingBronzeII);

        // Act
        var nextTier = RatingUtils.GetNextTier(belowBronzeIII);

        // Assert
        Assert.Equal(expectedTier, nextTier);
    }

    [Fact]
    public void GetNextTier_GivenAnyRatingAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingUtils.RatingEliteGrandmaster + 1;

        // Act
        var nextTier = RatingUtils.GetNextTier(aboveEliteGrandmaster);

        // Assert
        Assert.Null(nextTier);
    }

    [Theory]
    [InlineData(RatingUtils.RatingBronzeII)]
    [InlineData(RatingUtils.RatingBronzeI)]
    [InlineData(RatingUtils.RatingSilverIII)]
    [InlineData(RatingUtils.RatingSilverII)]
    [InlineData(RatingUtils.RatingSilverI)]
    [InlineData(RatingUtils.RatingGoldIII)]
    [InlineData(RatingUtils.RatingGoldII)]
    [InlineData(RatingUtils.RatingGoldI)]
    [InlineData(RatingUtils.RatingPlatinumIII)]
    [InlineData(RatingUtils.RatingPlatinumII)]
    [InlineData(RatingUtils.RatingPlatinumI)]
    [InlineData(RatingUtils.RatingEmeraldIII)]
    [InlineData(RatingUtils.RatingEmeraldII)]
    [InlineData(RatingUtils.RatingEmeraldI)]
    [InlineData(RatingUtils.RatingDiamondIII)]
    [InlineData(RatingUtils.RatingDiamondII)]
    [InlineData(RatingUtils.RatingDiamondI)]
    [InlineData(RatingUtils.RatingMasterIII)]
    [InlineData(RatingUtils.RatingMasterII)]
    [InlineData(RatingUtils.RatingMasterI)]
    [InlineData(RatingUtils.RatingGrandmasterIII)]
    [InlineData(RatingUtils.RatingGrandmasterII)]
    [InlineData(RatingUtils.RatingGrandmasterI)]
    [InlineData(RatingUtils.RatingEliteGrandmaster)]
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
    public void GetNextTierRating_GivenAnyBelowBronzeIII_ReturnsBronzeII()
    {
        // Arrange
        const double belowBronzeIII = RatingUtils.RatingBronzeIII + 1;
        const double expectedRating = RatingUtils.RatingBronzeII;

        // Act
        var nextTierRating = RatingUtils.GetNextTierRating(belowBronzeIII);

        // Assert
        Assert.Equal(expectedRating, nextTierRating);
    }

    [Fact]
    public void GetNextTierRating_GivenAnyAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingUtils.RatingEliteGrandmaster + 1;

        // Act
        var nextTierRating = RatingUtils.GetNextTierRating(aboveEliteGrandmaster);

        // Assert
        Assert.Null(nextTierRating);
    }

    [Theory]
    [InlineData(RatingUtils.RatingBronzeIII, -5)]
    [InlineData(RatingUtils.RatingBronzeIII, 5)]
    [InlineData(RatingUtils.RatingBronzeII, 15)]
    [InlineData(RatingUtils.RatingBronzeI, 25)]
    [InlineData(RatingUtils.RatingSilverIII, 23)]
    [InlineData(RatingUtils.RatingSilverII, 45)]
    [InlineData(RatingUtils.RatingSilverI, 54)]
    [InlineData(RatingUtils.RatingGoldIII, 52)]
    [InlineData(RatingUtils.RatingGoldII, 24)]
    [InlineData(RatingUtils.RatingGoldI, 32)]
    [InlineData(RatingUtils.RatingPlatinumIII, 5)]
    [InlineData(RatingUtils.RatingPlatinumII, 5)]
    [InlineData(RatingUtils.RatingPlatinumI, 5)]
    [InlineData(RatingUtils.RatingEmeraldIII, 5)]
    [InlineData(RatingUtils.RatingEmeraldII, 5)]
    [InlineData(RatingUtils.RatingEmeraldI, 5)]
    [InlineData(RatingUtils.RatingDiamondIII, 5)]
    [InlineData(RatingUtils.RatingDiamondII, 5)]
    [InlineData(RatingUtils.RatingDiamondI, 5)]
    [InlineData(RatingUtils.RatingMasterIII, 5)]
    [InlineData(RatingUtils.RatingMasterII, 5)]
    [InlineData(RatingUtils.RatingMasterI, 5)]
    [InlineData(RatingUtils.RatingGrandmasterIII, 5)]
    [InlineData(RatingUtils.RatingGrandmasterII, 5)]
    [InlineData(RatingUtils.RatingGrandmasterI, 5)]
    public void GetNextTierRatingDelta_ReturnsCorrectDelta(double rating, double offset)
    {
        // Arrange
        var ratingNextTier = RatingUtils.GetNextTierRating(rating + offset);
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
        const double ratingAboveEliteGrandmaster = RatingUtils.RatingEliteGrandmaster + 1;
        const int expectedDelta = 0;

        // Act
        var nextTierRatingDelta = RatingUtils.GetNextTierRatingDelta(ratingAboveEliteGrandmaster);

        // Assert
        Assert.Equal(expectedDelta, nextTierRatingDelta);
    }

    [Theory]
    [InlineData(RatingUtils.RatingBronzeIII)]
    [InlineData(RatingUtils.RatingBronzeII)]
    [InlineData(RatingUtils.RatingBronzeI)]
    [InlineData(RatingUtils.RatingSilverIII)]
    [InlineData(RatingUtils.RatingSilverII)]
    [InlineData(RatingUtils.RatingSilverI)]
    [InlineData(RatingUtils.RatingGoldIII)]
    [InlineData(RatingUtils.RatingGoldII)]
    [InlineData(RatingUtils.RatingGoldI)]
    [InlineData(RatingUtils.RatingPlatinumIII)]
    [InlineData(RatingUtils.RatingPlatinumII)]
    [InlineData(RatingUtils.RatingPlatinumI)]
    [InlineData(RatingUtils.RatingEmeraldIII)]
    [InlineData(RatingUtils.RatingEmeraldII)]
    [InlineData(RatingUtils.RatingEmeraldI)]
    [InlineData(RatingUtils.RatingDiamondIII)]
    [InlineData(RatingUtils.RatingDiamondII)]
    [InlineData(RatingUtils.RatingDiamondI)]
    [InlineData(RatingUtils.RatingMasterIII)]
    [InlineData(RatingUtils.RatingMasterII)]
    [InlineData(RatingUtils.RatingMasterI)]
    [InlineData(RatingUtils.RatingGrandmasterIII)]
    [InlineData(RatingUtils.RatingGrandmasterII)]
    [InlineData(RatingUtils.RatingGrandmasterI)]
    [InlineData(RatingUtils.RatingEliteGrandmaster)]
    public void GetPreviousTierRating_ReturnsCorrectRating(double rating)
    {
        // Arrange
        var aboveRating = rating + 1;

        // Act
        var prevTierRating = RatingUtils.GetPreviousTierRating(aboveRating);

        // Assert
        Assert.Equal(rating, prevTierRating);
    }

    [Fact]
    public void GetPreviousTierRating_GivenAnyBelowBronzeIII_ReturnsZero()
    {
        // Arrange
        const double ratingBelowBronzeIII = RatingUtils.RatingBronzeIII - 1;
        const double expectedRating = 0;

        // Act
        var previousTierRating = RatingUtils.GetPreviousTierRating(ratingBelowBronzeIII);

        // Assert
        Assert.Equal(expectedRating, previousTierRating);
    }

    [Theory]
    [InlineData(RatingUtils.RatingSilverIII)]
    [InlineData(RatingUtils.RatingGoldIII)]
    [InlineData(RatingUtils.RatingPlatinumIII)]
    [InlineData(RatingUtils.RatingEmeraldIII)]
    [InlineData(RatingUtils.RatingDiamondIII)]
    [InlineData(RatingUtils.RatingMasterIII)]
    [InlineData(RatingUtils.RatingGrandmasterIII)]
    [InlineData(RatingUtils.RatingEliteGrandmaster)]
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
    public void GetNextMajorTier_GivenAnyRatingBelowBronzeIII_ReturnsSilverIII()
    {
        // Arrange
        const double ratingBelowBronzeIII = RatingUtils.RatingBronzeIII - 1;
        var expectedTier = RatingUtils.GetTier(RatingUtils.RatingSilverIII);

        // Act
        var nextMajorTier = RatingUtils.GetNextMajorTier(ratingBelowBronzeIII);

        // Assert
        Assert.Equal(expectedTier, nextMajorTier);
    }

    [Fact]
    public void GetNextMajorTier_GivenAnyRatingAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double ratingAboveEliteGrandmaster = RatingUtils.RatingEliteGrandmaster + 1;

        // Act
        var nextMajorTier = RatingUtils.GetNextMajorTier(ratingAboveEliteGrandmaster);

        // Assert
        Assert.Null(nextMajorTier);
    }

    [Theory]
    [InlineData(RatingUtils.RatingSilverIII)]
    [InlineData(RatingUtils.RatingGoldIII)]
    [InlineData(RatingUtils.RatingPlatinumIII)]
    [InlineData(RatingUtils.RatingEmeraldIII)]
    [InlineData(RatingUtils.RatingDiamondIII)]
    [InlineData(RatingUtils.RatingMasterIII)]
    [InlineData(RatingUtils.RatingGrandmasterIII)]
    [InlineData(RatingUtils.RatingEliteGrandmaster)]
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
    public void GetNextMajorTierRating_GivenAnyBelowBronzeIII_ReturnsSilverIII()
    {
        // Arrange
        const double ratingBelowBronzeIII = RatingUtils.RatingBronzeIII - 1;
        const double expectedRating = RatingUtils.RatingSilverIII;

        // Act
        var nextMajorTierRating = RatingUtils.GetNextMajorTierRating(ratingBelowBronzeIII);

        // Assert
        Assert.Equal(expectedRating, nextMajorTierRating);
    }

    [Fact]
    public void GetNextMajorTierRating_GivenAnyAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingUtils.RatingEliteGrandmaster + 1;

        // Act
        var nextMajorTier = RatingUtils.GetNextMajorTierRating(aboveEliteGrandmaster);

        // Assert
        Assert.Null(nextMajorTier);
    }

    [Theory]
    [InlineData(RatingUtils.RatingBronzeIII, -5)]
    [InlineData(RatingUtils.RatingBronzeIII, 5)]
    [InlineData(RatingUtils.RatingBronzeII, 15)]
    [InlineData(RatingUtils.RatingBronzeI, 25)]
    [InlineData(RatingUtils.RatingSilverIII, 23)]
    [InlineData(RatingUtils.RatingSilverII, 45)]
    [InlineData(RatingUtils.RatingSilverI, 54)]
    [InlineData(RatingUtils.RatingGoldIII, 52)]
    [InlineData(RatingUtils.RatingGoldII, 24)]
    [InlineData(RatingUtils.RatingGoldI, 32)]
    [InlineData(RatingUtils.RatingPlatinumIII, 5)]
    [InlineData(RatingUtils.RatingPlatinumII, 5)]
    [InlineData(RatingUtils.RatingPlatinumI, 5)]
    [InlineData(RatingUtils.RatingEmeraldIII, 5)]
    [InlineData(RatingUtils.RatingEmeraldII, 5)]
    [InlineData(RatingUtils.RatingEmeraldI, 5)]
    [InlineData(RatingUtils.RatingDiamondIII, 5)]
    [InlineData(RatingUtils.RatingDiamondII, 5)]
    [InlineData(RatingUtils.RatingDiamondI, 5)]
    [InlineData(RatingUtils.RatingMasterIII, 5)]
    [InlineData(RatingUtils.RatingMasterII, 5)]
    [InlineData(RatingUtils.RatingMasterI, 5)]
    [InlineData(RatingUtils.RatingGrandmasterIII, 5)]
    [InlineData(RatingUtils.RatingGrandmasterII, 5)]
    [InlineData(RatingUtils.RatingGrandmasterI, 5)]
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
        const double ratingAboveEliteGrandmaster = RatingUtils.RatingEliteGrandmaster + 1;
        const int expectedDelta = 0;

        // Act
        var nextTierRatingDelta = RatingUtils.GetNextTierRatingDelta(ratingAboveEliteGrandmaster);

        // Assert
        Assert.Equal(expectedDelta, nextTierRatingDelta);
    }

    [Theory]
    [InlineData(RatingUtils.RatingBronzeIII)]
    [InlineData(RatingUtils.RatingSilverIII)]
    [InlineData(RatingUtils.RatingGoldIII)]
    [InlineData(RatingUtils.RatingPlatinumIII)]
    [InlineData(RatingUtils.RatingEmeraldIII)]
    [InlineData(RatingUtils.RatingDiamondIII)]
    [InlineData(RatingUtils.RatingMasterIII)]
    [InlineData(RatingUtils.RatingGrandmasterIII)]
    [InlineData(RatingUtils.RatingEliteGrandmaster)]
    public void GetMajorTierRating_ReturnsCorrectRating(double rating)
    {
        // Arrange
        var aboveRating = rating + 1;

        // Act
        var majorTierRating = RatingUtils.GetMajorTierRating(aboveRating);

        // Assert
        Assert.Equal(rating, majorTierRating);
    }

    [Fact]
    public void GetMajorTierRating_GivenAnyBelowBronzeIII_ReturnsZero()
    {
        // Arrange
        const double ratingBelowBronzeIII = RatingUtils.RatingBronzeIII - 1;
        const double expectedRating = 0;

        // Act
        var majorTierRating = RatingUtils.GetMajorTierRating(ratingBelowBronzeIII);

        // Assert
        Assert.Equal(expectedRating, majorTierRating);
    }

    [Theory]
    [InlineData(RatingUtils.RatingBronzeIII, -5)]
    [InlineData(RatingUtils.RatingBronzeIII, 5)]
    [InlineData(RatingUtils.RatingBronzeII, 15)]
    [InlineData(RatingUtils.RatingBronzeI, 25)]
    [InlineData(RatingUtils.RatingSilverIII, 23)]
    [InlineData(RatingUtils.RatingSilverII, 45)]
    [InlineData(RatingUtils.RatingSilverI, 54)]
    [InlineData(RatingUtils.RatingGoldIII, 52)]
    [InlineData(RatingUtils.RatingGoldII, 24)]
    [InlineData(RatingUtils.RatingGoldI, 32)]
    [InlineData(RatingUtils.RatingPlatinumIII, 31)]
    [InlineData(RatingUtils.RatingPlatinumII, 29)]
    [InlineData(RatingUtils.RatingPlatinumI, 22)]
    [InlineData(RatingUtils.RatingEmeraldIII, 14)]
    [InlineData(RatingUtils.RatingEmeraldII, 19)]
    [InlineData(RatingUtils.RatingEmeraldI, 20)]
    [InlineData(RatingUtils.RatingDiamondIII, 13)]
    [InlineData(RatingUtils.RatingDiamondII, 17)]
    [InlineData(RatingUtils.RatingDiamondI, 29)]
    [InlineData(RatingUtils.RatingMasterIII, 26)]
    [InlineData(RatingUtils.RatingMasterII, 15)]
    [InlineData(RatingUtils.RatingMasterI, 5)]
    [InlineData(RatingUtils.RatingGrandmasterIII, 10)]
    [InlineData(RatingUtils.RatingGrandmasterII, 8)]
    [InlineData(RatingUtils.RatingGrandmasterI, 16)]
    public void GetNextTierFillPercentage_ReturnsCorrectPercentage(double rating, double offset)
    {
        // Arrange
        var nextTierRating = RatingUtils.GetNextTierRating(rating + offset);
        var prevTierRating = RatingUtils.GetPreviousTierRating(rating + offset);
        var expectedPercentage = (rating - prevTierRating + offset) / (nextTierRating - prevTierRating);

        // Act
        var actualPercentage = RatingUtils.GetNextTierFillPercentage(rating + offset);

        // Assert
        Assert.Equal(expectedPercentage, actualPercentage);
    }

    [Fact]
    public void GetNextTierFillPercentage_GivenAnyAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingUtils.RatingEliteGrandmaster + 1;

        // Act
        var percentage = RatingUtils.GetNextTierFillPercentage(aboveEliteGrandmaster);

        // Assert
        Assert.Null(percentage);
    }

    [Theory]
    [InlineData(RatingUtils.RatingBronzeIII, -23)]
    [InlineData(RatingUtils.RatingBronzeIII, 5)]
    [InlineData(RatingUtils.RatingBronzeII, 15)]
    [InlineData(RatingUtils.RatingBronzeI, 25)]
    [InlineData(RatingUtils.RatingSilverIII, 23)]
    [InlineData(RatingUtils.RatingSilverII, 45)]
    [InlineData(RatingUtils.RatingSilverI, 54)]
    [InlineData(RatingUtils.RatingGoldIII, 52)]
    [InlineData(RatingUtils.RatingGoldII, 24)]
    [InlineData(RatingUtils.RatingGoldI, 32)]
    [InlineData(RatingUtils.RatingPlatinumIII, 31)]
    [InlineData(RatingUtils.RatingPlatinumII, 29)]
    [InlineData(RatingUtils.RatingPlatinumI, 22)]
    [InlineData(RatingUtils.RatingEmeraldIII, 14)]
    [InlineData(RatingUtils.RatingEmeraldII, 19)]
    [InlineData(RatingUtils.RatingEmeraldI, 20)]
    [InlineData(RatingUtils.RatingDiamondIII, 13)]
    [InlineData(RatingUtils.RatingDiamondII, 17)]
    [InlineData(RatingUtils.RatingDiamondI, 29)]
    [InlineData(RatingUtils.RatingMasterIII, 26)]
    [InlineData(RatingUtils.RatingMasterII, 15)]
    [InlineData(RatingUtils.RatingMasterI, 5)]
    [InlineData(RatingUtils.RatingGrandmasterIII, 10)]
    [InlineData(RatingUtils.RatingGrandmasterII, 8)]
    [InlineData(RatingUtils.RatingGrandmasterI, 16)]
    public void GetNextMajorTierFillPercentage_ReturnsCorrectPercentage(double rating, double offset)
    {
        // Arrange
        var nextMajorTierRating = RatingUtils.GetNextMajorTierRating(rating + offset);
        var currentMajorTierRating = RatingUtils.GetMajorTierRating(rating + offset);
        var expectedPercentage = (rating - currentMajorTierRating + offset) / (nextMajorTierRating - currentMajorTierRating);

        // Act
        var actualPercentage = RatingUtils.GetNextMajorTierFillPercentage(rating + offset);

        // Assert
        Assert.Equal(expectedPercentage, actualPercentage);
    }

    [Fact]
    public void GetNextMajorTierFillPercentage_GivenAboveEliteGrandmaster_ReturnsNull()
    {
        // Arrange
        const double aboveEliteGrandmaster = RatingUtils.RatingEliteGrandmaster + 1;

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
