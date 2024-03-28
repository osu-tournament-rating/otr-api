using API.Utilities;

namespace APITests.Tests.Utilities;

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
}
