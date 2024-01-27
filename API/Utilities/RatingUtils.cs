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

	public static int? GetCurrentSubTier(double rating) => rating switch
	{
		< RatingBronzeII => 3,
		< RatingBronzeI => 2,
		< RatingSilverIII => 1,
		< RatingSilverII => 3,
		< RatingSilverI => 2,
		< RatingGoldIII => 1,
		< RatingGoldII => 3,
		< RatingGoldI => 2,
		< RatingPlatinumIII => 1,
		< RatingPlatinumII => 3,
		< RatingPlatinumI => 2,
		< RatingEmeraldIII => 1,
		< RatingEmeraldII => 3,
		< RatingEmeraldI => 2,
		< RatingDiamondIII => 1,
		< RatingDiamondII => 3,
		< RatingDiamondI => 2,
		< RatingMasterIII => 1,
		< RatingMasterII => 3,
		< RatingMasterI => 2,
		< RatingGrandmasterIII => 1,
		< RatingGrandmasterII => 3,
		< RatingGrandmasterI => 2,
		< RatingEliteGrandmaster => 1,
		_ => null
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

	public static double GetRatingDeltaForNextTier(double rating) => rating switch
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

	public static double? GetRatingForNextTier(double rating) => rating switch
	{
		< RatingBronzeII => RatingBronzeII,
		< RatingBronzeI => RatingBronzeI,
		< RatingSilverIII => RatingSilverIII,
		< RatingSilverII => RatingSilverII,
		< RatingSilverI => RatingSilverI,
		< RatingGoldIII => RatingGoldIII,
		< RatingGoldII => RatingGoldII,
		< RatingGoldI => RatingGoldI,
		< RatingPlatinumIII => RatingPlatinumIII,
		< RatingPlatinumII => RatingPlatinumII,
		< RatingPlatinumI => RatingPlatinumI,
		< RatingEmeraldIII => RatingEmeraldIII,
		< RatingEmeraldII => RatingEmeraldII,
		< RatingEmeraldI => RatingEmeraldI,
		< RatingDiamondIII => RatingDiamondIII,
		< RatingDiamondII => RatingDiamondII,
		< RatingDiamondI => RatingDiamondI,
		< RatingMasterIII => RatingMasterIII,
		< RatingMasterII => RatingMasterII,
		< RatingMasterI => RatingMasterI,
		< RatingGrandmasterIII => RatingGrandmasterIII,
		< RatingGrandmasterII => RatingGrandmasterII,
		< RatingGrandmasterI => RatingGrandmasterI,
		< RatingEliteGrandmaster => RatingEliteGrandmaster,
		_ => null
	};

	public static double? GetRatingForPreviousTier(double rating) => rating switch
	{
		< RatingBronzeIII => null,
		< RatingBronzeII => RatingBronzeIII,
		< RatingBronzeI => RatingBronzeII,
		< RatingSilverIII => RatingBronzeI,
		< RatingSilverII => RatingSilverIII,
		< RatingSilverI => RatingSilverII,
		< RatingGoldIII => RatingSilverI,
		< RatingGoldII => RatingGoldIII,
		< RatingGoldI => RatingGoldII,
		< RatingPlatinumIII => RatingGoldI,
		< RatingPlatinumII => RatingPlatinumIII,
		< RatingPlatinumI => RatingPlatinumII,
		< RatingEmeraldIII => RatingPlatinumI,
		< RatingEmeraldII => RatingEmeraldIII,
		< RatingEmeraldI => RatingEmeraldII,
		< RatingDiamondIII => RatingEmeraldI,
		< RatingDiamondII => RatingDiamondIII,
		< RatingDiamondI => RatingDiamondII,
		< RatingMasterIII => RatingDiamondI,
		< RatingMasterII => RatingMasterIII,
		< RatingMasterI => RatingMasterII,
		< RatingGrandmasterIII => RatingMasterI,
		< RatingGrandmasterII => RatingGrandmasterIII,
		< RatingGrandmasterI => RatingGrandmasterII,
		< RatingEliteGrandmaster => RatingGrandmasterI,
		_ => RatingEliteGrandmaster
	};

	public static double GetRatingForNextMajorTier(double rating) => rating switch
	{
		< RatingSilverIII => RatingSilverIII,
		< RatingGoldIII => RatingGoldIII,
		< RatingPlatinumIII => RatingPlatinumIII,
		< RatingEmeraldIII => RatingEmeraldIII,
		< RatingDiamondIII => RatingDiamondIII,
		< RatingMasterIII => RatingMasterIII,
		< RatingGrandmasterIII => RatingGrandmasterIII,
		< RatingEliteGrandmaster => RatingEliteGrandmaster,
		_ => 0
	};

	public static double GetRatingDeltaForNextMajorTier(double rating) => rating switch
	{
		< RatingSilverIII => RatingSilverIII - rating,
		< RatingGoldIII => RatingGoldIII - rating,
		< RatingPlatinumIII => RatingPlatinumIII - rating,
		< RatingEmeraldIII => RatingEmeraldIII - rating,
		< RatingDiamondIII => RatingDiamondIII - rating,
		< RatingMasterIII => RatingMasterIII - rating,
		< RatingGrandmasterIII => RatingGrandmasterIII - rating,
		< RatingEliteGrandmaster => RatingEliteGrandmaster - rating,
		_ => 0
	};

	public static string? GetNextMajorTier(double rating) => rating switch
	{
		< RatingSilverIII => GetTier(RatingSilverIII),
		< RatingGoldIII => GetTier(RatingGoldIII),
		< RatingPlatinumIII => GetTier(RatingPlatinumIII),
		< RatingEmeraldIII => GetTier(RatingEmeraldIII),
		< RatingDiamondIII => GetTier(RatingDiamondIII),
		< RatingMasterIII => GetTier(RatingMasterIII),
		< RatingGrandmasterIII => GetTier(RatingGrandmasterIII),
		< RatingEliteGrandmaster => GetTier(RatingEliteGrandmaster),
		_ => null
	};

	public static double? GetMinimumRatingBeforeMajorTierFalloff(double rating) => rating switch
	{
		< RatingBronzeI => null,
		< RatingGoldIII => RatingSilverIII,
		< RatingPlatinumIII => RatingGoldIII,
		< RatingEmeraldIII => RatingPlatinumIII,
		< RatingDiamondIII => RatingEmeraldIII,
		< RatingMasterIII => RatingDiamondIII,
		< RatingGrandmasterIII => RatingMasterIII,
		< RatingEliteGrandmaster => RatingGrandmasterIII,
		>= RatingEliteGrandmaster => RatingEliteGrandmaster,
		_ => null
	};

	public static double? GetSubTierFillPercentage(double rating)
	{
		double? minRating = GetRatingForPreviousTier(rating);
		double? maxRating = GetRatingForNextTier(rating);

		if (minRating == null || maxRating == null)
		{
			return null;
		}

		return (rating - minRating.Value) / (maxRating - minRating.Value);
	}

	public static double? GetMajorTierFillPercentage(double rating)
	{
		double? minRating = GetMinimumRatingBeforeMajorTierFalloff(rating);
		double maxRating = GetRatingForNextMajorTier(rating);
		if (minRating == null || maxRating == 0)
		{
			return null;
		}

		return (rating - minRating.Value) / (maxRating - minRating.Value);
	}

	public static bool IsProvisional(double volatility, int matchesPlayed, int tournamentsPlayed) => volatility >= 200.0 || matchesPlayed <= 8 || tournamentsPlayed <= 2;
}