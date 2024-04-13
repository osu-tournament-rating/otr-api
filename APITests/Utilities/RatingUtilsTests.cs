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
    public void GetTier_ReturnsCorrectTier_GivenRatingRanges(double rating, string expectedTier)
    {
        // Arrange

        // Act
        var actualTier = RatingUtils.GetTier(rating);

        // Assert
        Assert.Equal(expectedTier, actualTier);
    }

    [Theory]
    [InlineData(RatingUtils.RatingBronzeIII, "Bronze II")]
    [InlineData(RatingUtils.RatingBronzeII, "Bronze I")]
    [InlineData(RatingUtils.RatingBronzeI, "Silver III")]
    [InlineData(RatingUtils.RatingSilverIII, "Silver II")]
    [InlineData(RatingUtils.RatingSilverII, "Silver I")]
    [InlineData(RatingUtils.RatingSilverI, "Gold III")]
    [InlineData(RatingUtils.RatingGoldIII, "Gold II")]
    [InlineData(RatingUtils.RatingGoldII, "Gold I")]
    [InlineData(RatingUtils.RatingGoldI, "Platinum III")]
    [InlineData(RatingUtils.RatingPlatinumIII, "Platinum II")]
    [InlineData(RatingUtils.RatingPlatinumII, "Platinum I")]
    [InlineData(RatingUtils.RatingPlatinumI, "Emerald III")]
    [InlineData(RatingUtils.RatingEmeraldIII, "Emerald II")]
    [InlineData(RatingUtils.RatingEmeraldII, "Emerald I")]
    [InlineData(RatingUtils.RatingEmeraldI, "Diamond III")]
    [InlineData(RatingUtils.RatingDiamondIII, "Diamond II")]
    [InlineData(RatingUtils.RatingDiamondII, "Diamond I")]
    [InlineData(RatingUtils.RatingDiamondI, "Master III")]
    [InlineData(RatingUtils.RatingMasterIII, "Master II")]
    [InlineData(RatingUtils.RatingMasterII, "Master I")]
    [InlineData(RatingUtils.RatingMasterI, "Grandmaster III")]
    [InlineData(RatingUtils.RatingGrandmasterIII, "Grandmaster II")]
    [InlineData(RatingUtils.RatingGrandmasterII, "Grandmaster I")]
    [InlineData(RatingUtils.RatingGrandmasterI, "Elite Grandmaster")]
    [InlineData(RatingUtils.RatingEliteGrandmaster, "Elite Grandmaster")]
    public void GetNextTier_ReturnsNextTier_GivenCurrentTier(double currentRating, string expectedNextTier)
    {
        // Arrange

        // Act
        var actualNextTier = RatingUtils.GetNextTier(currentRating);

        // Assert
        Assert.Equal(expectedNextTier, actualNextTier);
    }

    [Theory]
    [InlineData(200.0, 20, 1)]
    [InlineData(200.0, 20, 3)]
    [InlineData(201, 20, 4)]
    [InlineData(50, 5, 2)]
    [InlineData(82, 20, 2)]
    [InlineData(82, 20, 1)]
    public void ProvisionalFlag_IsTrue_WhenProvisional(
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
    public void ProvisionalFlag_IsFalse_WhenNotProvisional(
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
    [InlineData(RatingUtils.RatingBronzeIII - 5)] // Only null case
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
    public void GetPreviousTier_ReturnsProperPreviousTier(double rating)
    {
        // Arrange
        var aboveRating = rating + 1;

        // Act
        var prevTier = RatingUtils.GetRatingForPreviousTier(aboveRating);

        // Assert
        // Ratings below Bronze 3 have no previous tier
        if (rating < RatingUtils.RatingBronzeIII)
        {
            Assert.Null(prevTier);
        }
        else
        {
            Assert.Equal(rating, prevTier);
        }
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
    [InlineData(RatingUtils.RatingEliteGrandmaster, null)]
    public void GetSubTier_ReturnsSubTier_GivenCurrentTier(double rating, int? expectedSubTier)
    {
        // Arrange

        // Act
        var subTier = RatingUtils.GetCurrentSubTier(rating);

        // Assert
        Assert.Equal(expectedSubTier, subTier);
    }

    [Theory]
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
    [InlineData(RatingUtils.RatingEliteGrandmaster, 5)]
    public void GetRatingDelta_ReturnsProperDelta(double rating, double offset)
    {
        // Arrange
        var ratingNextTier = RatingUtils.GetRatingForNextTier(rating);
        var expected = ratingNextTier - rating - offset;

        // Act
        var delta = RatingUtils.GetRatingDeltaForNextTier(rating + offset);

        // Assert
        // Elite Grandmaster and higher have zero delta as there is no higher tier
        if (rating >= RatingUtils.RatingEliteGrandmaster)
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(0, delta);
                Assert.Null(ratingNextTier);
            });
        }
        else
        {
            Assert.Equal(expected, delta);
        }
    }

    [Theory]
    [InlineData(RatingUtils.RatingBronzeIII, RatingUtils.RatingBronzeII)]
    [InlineData(RatingUtils.RatingBronzeII, RatingUtils.RatingBronzeI)]
    [InlineData(RatingUtils.RatingBronzeI, RatingUtils.RatingSilverIII)]
    [InlineData(RatingUtils.RatingSilverIII, RatingUtils.RatingSilverII)]
    [InlineData(RatingUtils.RatingSilverII, RatingUtils.RatingSilverI)]
    [InlineData(RatingUtils.RatingSilverI, RatingUtils.RatingGoldIII)]
    [InlineData(RatingUtils.RatingGoldIII, RatingUtils.RatingGoldII)]
    [InlineData(RatingUtils.RatingGoldII, RatingUtils.RatingGoldI)]
    [InlineData(RatingUtils.RatingGoldI, RatingUtils.RatingPlatinumIII)]
    [InlineData(RatingUtils.RatingPlatinumIII, RatingUtils.RatingPlatinumII)]
    [InlineData(RatingUtils.RatingPlatinumII, RatingUtils.RatingPlatinumI)]
    [InlineData(RatingUtils.RatingPlatinumI, RatingUtils.RatingEmeraldIII)]
    [InlineData(RatingUtils.RatingEmeraldIII, RatingUtils.RatingEmeraldII)]
    [InlineData(RatingUtils.RatingEmeraldII, RatingUtils.RatingEmeraldI)]
    [InlineData(RatingUtils.RatingEmeraldI, RatingUtils.RatingDiamondIII)]
    [InlineData(RatingUtils.RatingDiamondIII, RatingUtils.RatingDiamondII)]
    [InlineData(RatingUtils.RatingDiamondII, RatingUtils.RatingDiamondI)]
    [InlineData(RatingUtils.RatingDiamondI, RatingUtils.RatingMasterIII)]
    [InlineData(RatingUtils.RatingMasterIII, RatingUtils.RatingMasterII)]
    [InlineData(RatingUtils.RatingMasterII, RatingUtils.RatingMasterI)]
    [InlineData(RatingUtils.RatingMasterI, RatingUtils.RatingGrandmasterIII)]
    [InlineData(RatingUtils.RatingGrandmasterIII, RatingUtils.RatingGrandmasterII)]
    [InlineData(RatingUtils.RatingGrandmasterII, RatingUtils.RatingGrandmasterI)]
    [InlineData(RatingUtils.RatingGrandmasterI, RatingUtils.RatingEliteGrandmaster)]
    [InlineData(RatingUtils.RatingEliteGrandmaster, null)]
    public void GetRatingForNextTier_ReturnsRatingForNextTier_GivenCurrentRating(
        double rating,
        double? expectedNextTier
    )
    {
        // Arrange

        // Act
        var nextTier = RatingUtils.GetRatingForNextTier(rating);

        // Assert
        Assert.Equal(expectedNextTier, nextTier);
    }
}
