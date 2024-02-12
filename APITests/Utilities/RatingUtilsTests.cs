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
		string actualTier = RatingUtils.GetTier(rating);

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
		string actualNextTier = RatingUtils.GetNextTier(currentRating);

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
	public void ProvisionalFlag_IsTrue_WhenProvisional(double volatility, int matchesPlayed, int tournamentsPlayed)
	{
		// Arrange

		// Act
		bool isProvisional = RatingUtils.IsProvisional(volatility, matchesPlayed, tournamentsPlayed);

		// Assert
		Assert.True(isProvisional);
	}

	[Theory]
	[InlineData(199.9, 10, 3)]
	[InlineData(50, 20, 4)]
	[InlineData(21, 40, 3)]
	public void ProvisionalFlag_IsFalse_WhenNotProvisional(double volatility, int matchesPlayed, int tournamentsPlayed)
	{
		// Arrange

		// Act
		bool isProvisional = RatingUtils.IsProvisional(volatility, matchesPlayed, tournamentsPlayed);

		// Assert
		Assert.False(isProvisional);
	}
	
	[Theory]
	[InlineData(RatingUtils.RatingBronzeIII - 1, null)]
	[InlineData(RatingUtils.RatingBronzeIII + 1, RatingUtils.RatingBronzeIII)]
	[InlineData(RatingUtils.RatingBronzeII + 1, RatingUtils.RatingBronzeII)]
	[InlineData(RatingUtils.RatingBronzeI + 1, RatingUtils.RatingBronzeI)]
	[InlineData(RatingUtils.RatingSilverIII + 1, RatingUtils.RatingSilverIII)]
	[InlineData(RatingUtils.RatingSilverII + 1, RatingUtils.RatingSilverII)]
	[InlineData(RatingUtils.RatingSilverI + 1, RatingUtils.RatingSilverI)]
	[InlineData(RatingUtils.RatingGoldIII + 1, RatingUtils.RatingGoldIII)]
	[InlineData(RatingUtils.RatingGoldII + 1, RatingUtils.RatingGoldII)]
	[InlineData(RatingUtils.RatingGoldI + 1, RatingUtils.RatingGoldI)]
	[InlineData(RatingUtils.RatingPlatinumIII + 1, RatingUtils.RatingPlatinumIII)]
	[InlineData(RatingUtils.RatingPlatinumII + 1, RatingUtils.RatingPlatinumII)]
	[InlineData(RatingUtils.RatingPlatinumI + 1, RatingUtils.RatingPlatinumI)]
	[InlineData(RatingUtils.RatingEmeraldIII + 1, RatingUtils.RatingEmeraldIII)]
	[InlineData(RatingUtils.RatingEmeraldII + 1, RatingUtils.RatingEmeraldII)]
	[InlineData(RatingUtils.RatingEmeraldI + 1, RatingUtils.RatingEmeraldI)]
	[InlineData(RatingUtils.RatingDiamondIII + 1, RatingUtils.RatingDiamondIII)]
	[InlineData(RatingUtils.RatingDiamondII + 1, RatingUtils.RatingDiamondII)]
	[InlineData(RatingUtils.RatingDiamondI + 1, RatingUtils.RatingDiamondI)]
	[InlineData(RatingUtils.RatingMasterIII + 1, RatingUtils.RatingMasterIII)]
	[InlineData(RatingUtils.RatingMasterII + 1, RatingUtils.RatingMasterII)]
	[InlineData(RatingUtils.RatingMasterI + 1, RatingUtils.RatingMasterI)]
	[InlineData(RatingUtils.RatingGrandmasterIII + 1, RatingUtils.RatingGrandmasterIII)]
	[InlineData(RatingUtils.RatingGrandmasterII + 1, RatingUtils.RatingGrandmasterII)]
	[InlineData(RatingUtils.RatingGrandmasterI + 1, RatingUtils.RatingGrandmasterI)]
	[InlineData(RatingUtils.RatingEliteGrandmaster + 1, RatingUtils.RatingEliteGrandmaster)]
	public void GetPreviousTier_ReturnsCorrectTier_GivenCurrentTier(double rating, double? expectedPrevTier)
	{
		// Arrange

		// Act
		double? prevTier = RatingUtils.GetRatingForPreviousTier(rating);
		
		// Assert
		Assert.Equal(expectedPrevTier, prevTier);
		
	}
	
}