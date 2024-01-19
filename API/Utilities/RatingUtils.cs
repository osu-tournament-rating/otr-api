namespace API.Utilities;

public static class RatingUtils
{
	// Players at or below the threshold are considered to be in the previous tier

	// Bronze: 100-499
	public const double RatingBronzeIII = 100;
	public const double RatingBronzeII = 250;
	public const double RatingBronzeI = 400;
	public const double RatingSilverIII = 500;
	public const double RatingSilverII = 600;
	public const double RatingSilverI = 700;
	public const double RatingGoldIII = 800;
	public const double RatingGoldII = 900;
	public const double RatingGoldI = 1000;
	public const double RatingPlatinumIII = 1100;
	public const double RatingPlatinumII = 1200;
	public const double RatingPlatinumI = 1300;
	public const double RatingEmeraldIII = 1400;
	public const double RatingEmeraldII = 1500;
	public const double RatingEmeraldI = 1600;
	public const double RatingDiamondIII = 1700;
	public const double RatingDiamondII = 1800;
	public const double RatingDiamondI = 1900;
	public const double RatingMasterIII = 2000;
	public const double RatingMasterII = 2100;
	public const double RatingMasterI = 2200;
	public const double RatingGrandmasterIII = 2300;
	public const double RatingGrandmasterII = 2400;
	public const double RatingGrandmasterI = 2500;
	public const double RatingEliteGrandmaster = 2600;

	public static string GetTier(double rating) => rating switch
	{
		< RatingBronzeII => "Bronze III",
		< RatingBronzeI => "Bronze II",
		< RatingSilverIII => "Bronze I",
		< RatingSilverII => "Silver III",
		< RatingSilverI => "Silver II",
		< RatingGoldIII => "Silver I",
		< RatingGoldII => "Gold III",
		< RatingGoldI => "Gold II",
		< RatingPlatinumIII => "Gold I",
		< RatingPlatinumII => "Platinum III",
		< RatingPlatinumI => "Platinum II",
		< RatingEmeraldIII => "Platinum I",
		< RatingEmeraldII => "Emerald III",
		< RatingEmeraldI => "Emerald II",
		< RatingDiamondIII => "Emerald I",
		< RatingDiamondII => "Diamond III",
		< RatingDiamondI => "Diamond II",
		< RatingMasterIII => "Diamond I",
		< RatingMasterII => "Master III",
		< RatingMasterI => "Master II",
		< RatingGrandmasterIII => "Master I",
		< RatingGrandmasterII => "Grandmaster III",
		< RatingGrandmasterI => "Grandmaster II",
		< RatingEliteGrandmaster => "Grandmaster I",
		_ => "Elite Grandmaster"
	};

	public static string GetNextTier(double rating) => rating switch
	{
		< RatingBronzeII => GetTier(RatingBronzeII),
		< RatingBronzeI => GetTier(RatingBronzeI),
		< RatingSilverIII => GetTier(RatingSilverIII),
		< RatingSilverII => GetTier(RatingSilverII),
		< RatingSilverI => GetTier(RatingSilverI),
		< RatingGoldIII => GetTier(RatingGoldIII),
		< RatingGoldII => GetTier(RatingGoldII),
		< RatingGoldI => GetTier(RatingGoldI),
		< RatingPlatinumIII => GetTier(RatingPlatinumIII),
		< RatingPlatinumII => GetTier(RatingPlatinumII),
		< RatingPlatinumI => GetTier(RatingPlatinumI),
		< RatingEmeraldIII => GetTier(RatingEmeraldIII),
		< RatingEmeraldII => GetTier(RatingEmeraldII),
		< RatingEmeraldI => GetTier(RatingEmeraldI),
		< RatingDiamondIII => GetTier(RatingDiamondIII),
		< RatingDiamondII => GetTier(RatingDiamondII),
		< RatingDiamondI => GetTier(RatingDiamondI),
		< RatingMasterIII => GetTier(RatingMasterIII),
		< RatingMasterII => GetTier(RatingMasterII),
		< RatingMasterI => GetTier(RatingMasterI),
		< RatingGrandmasterIII => GetTier(RatingGrandmasterIII),
		< RatingGrandmasterII => GetTier(RatingGrandmasterII),
		< RatingGrandmasterI => GetTier(RatingGrandmasterI),
		< RatingEliteGrandmaster => GetTier(RatingEliteGrandmaster),
		_ => GetTier(RatingEliteGrandmaster)
	};

	public static double GetRatingForNextTier(double rating) => rating switch
	{
		< RatingBronzeII => RatingBronzeII - rating,
		< RatingBronzeI => RatingBronzeI - rating,
		< RatingSilverIII => RatingSilverIII - rating,
		< RatingSilverII => RatingSilverII - rating,
		< RatingSilverI => RatingSilverI - rating,
		< RatingGoldIII => RatingGoldIII - rating,
		< RatingGoldII => RatingGoldII - rating,
		< RatingGoldI => RatingGoldI - rating,
		< RatingPlatinumIII => RatingPlatinumIII - rating,
		< RatingPlatinumII => RatingPlatinumII - rating,
		< RatingPlatinumI => RatingPlatinumI - rating,
		< RatingEmeraldIII => RatingEmeraldIII - rating,
		< RatingEmeraldII => RatingEmeraldII - rating,
		< RatingEmeraldI => RatingEmeraldI - rating,
		< RatingDiamondIII => RatingDiamondIII - rating,
		< RatingDiamondII => RatingDiamondII - rating,
		< RatingDiamondI => RatingDiamondI - rating,
		< RatingMasterIII => RatingMasterIII - rating,
		< RatingMasterII => RatingMasterII - rating,
		< RatingMasterI => RatingMasterI - rating,
		< RatingGrandmasterIII => RatingGrandmasterIII - rating,
		< RatingGrandmasterII => RatingGrandmasterII - rating,
		< RatingGrandmasterI => RatingGrandmasterI - rating,
		< RatingEliteGrandmaster => RatingEliteGrandmaster - rating,
		_ => 0
	};

	// Useful for the front-end rating progress bar
	/// <summary>
	/// Returns the difference between the ratings required for the next tier and the current tier.
	/// This is used by the front-end's progress bar to determine how much progress has been made towards the next tier.
	/// </summary>
	/// <param name="rating"></param>
	/// <returns></returns>
	public static double GetMajorTierRatingDelta(double rating) => rating switch
	{
		< RatingBronzeI => RatingBronzeI - rating,
		< RatingSilverI => RatingSilverI - rating,
		< RatingGoldI => RatingGoldI - rating,
		< RatingPlatinumI => RatingPlatinumI - rating,
		< RatingEmeraldI => RatingEmeraldI - rating,
		< RatingDiamondI => RatingDiamondI - rating,
		< RatingMasterI => RatingMasterI - rating,
		< RatingGrandmasterI => RatingGrandmasterI - rating,
		< RatingEliteGrandmaster => RatingEliteGrandmaster - rating,
		_ => 0
	};

	public static bool IsProvisional(double volatility, int matchesPlayed, int tournamentsPlayed) => volatility >= 200.0 || matchesPlayed <= 8 || tournamentsPlayed <= 2;
}