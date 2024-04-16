namespace API.Utilities;

/// <summary>
/// Collection of helper functions to assist in parsing rating values
/// </summary>
public static class RatingUtils
{
    /**
     * Verbiage:
     *
     * Threshold - The lowest rating of any individual tier (i.e. RatingSilverIII => 300)
     * Players at or below a threshold are considered to be in the previous tier
     * A rating would not be considered to be of tier "Silver III" until it is greater than 300
     *
     * Tier - An individual rating threshold's string representation (i.e. RatingSilverIII => "Silver III")
     *
     * Major Tier - An individual "division" (i.e Silver, Gold, Platinum)
     *
     * Sub Tier - One of three "sub-divisions" for a major tier (i.e. SilverI, SilverII, SilverIII)
     */

    // Bronze: 100 - 300
    public const double RatingBronzeIII = 100;
    public const double RatingBronzeII = 165;
    public const double RatingBronzeI = 235;
    // Silver: 300 - 500
    public const double RatingSilverIII = 300;
    public const double RatingSilverII = 365;
    public const double RatingSilverI = 430;
    // Gold: 500 - 700
    public const double RatingGoldIII = 500;
    public const double RatingGoldII = 570;
    public const double RatingGoldI = 625;
    // Platinum: 700 - 900
    public const double RatingPlatinumIII = 700;
    public const double RatingPlatinumII = 770;
    public const double RatingPlatinumI = 825;
    // Emerald: 900 - 1200
    public const double RatingEmeraldIII = 900;
    public const double RatingEmeraldII = 1000;
    public const double RatingEmeraldI = 1100;
    // Diamond: 1200 - 1500
    public const double RatingDiamondIII = 1200;
    public const double RatingDiamondII = 1300;
    public const double RatingDiamondI = 1400;
    // Master: 1500 - 1900
    public const double RatingMasterIII = 1500;
    public const double RatingMasterII = 1625;
    public const double RatingMasterI = 1750;
    // Grandmaster: 1900 - 2500
    public const double RatingGrandmasterIII = 1900;
    public const double RatingGrandmasterII = 2100;
    public const double RatingGrandmasterI = 2300;
    // Elite Grandmaster: 2501+
    public const double RatingEliteGrandmaster = 2500;

    /// <summary>
    /// Gets the string representation of the given rating
    /// </summary>
    public static string GetTier(double rating) =>
        rating switch
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

    /// <summary>
    /// Gets the integer representation of the sub-tier for the given rating
    /// </summary>
    /// <remarks>Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static int? GetSubTier(double rating) =>
        rating switch
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

    /// <summary>
    /// Gets the string representation of the next tier for the given rating
    /// </summary>
    /// <remarks>Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static string? GetNextTier(double rating) =>
        rating switch
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
            _ => null
        };

    /// <summary>
    /// Gets the rating threshold of the next tier for the given rating
    /// </summary>
    /// <remarks>Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static double? GetNextTierRating(double rating) =>
        rating switch
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

    /// <summary>
    /// Gets the difference in rating of the next tier for the given rating
    /// </summary>
    /// <remarks>Will return 0 for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static double GetNextTierRatingDelta(double rating) =>
        rating switch
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

    /// <summary>
    /// Gets the rating threshold of the previous tier for the given rating
    /// </summary>
    public static double GetPreviousTierRating(double rating) =>
        rating switch
        {
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

    /// <summary>
    /// Gets the string representation of the next major tier for the given rating
    /// </summary>
    /// <remarks>Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static string? GetNextMajorTier(double rating) =>
        rating switch
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

    /// <summary>
    /// Gets the rating threshold of the next major tier for the given rating
    /// </summary>
    /// <remarks>Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static double? GetNextMajorTierRating(double rating) =>
        rating switch
        {
            < RatingSilverIII => RatingSilverIII,
            < RatingGoldIII => RatingGoldIII,
            < RatingPlatinumIII => RatingPlatinumIII,
            < RatingEmeraldIII => RatingEmeraldIII,
            < RatingDiamondIII => RatingDiamondIII,
            < RatingMasterIII => RatingMasterIII,
            < RatingGrandmasterIII => RatingGrandmasterIII,
            < RatingEliteGrandmaster => RatingEliteGrandmaster,
            _ => null
        };

    /// <summary>
    /// Gets the difference in rating of the next major tier for the given rating
    /// </summary>
    /// <remarks>Will return 0 for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static double GetNextMajorTierRatingDelta(double rating) =>
        rating switch
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

    /// <summary>
    /// Gets the rating threshold of the major tier for the given rating
    /// </summary>
    public static double GetMajorTierRating(double rating) =>
        rating switch
        {
            < RatingSilverIII => RatingBronzeIII,
            < RatingGoldIII => RatingSilverIII,
            < RatingPlatinumIII => RatingGoldIII,
            < RatingEmeraldIII => RatingPlatinumIII,
            < RatingDiamondIII => RatingEmeraldIII,
            < RatingMasterIII => RatingDiamondIII,
            < RatingGrandmasterIII => RatingMasterIII,
            < RatingEliteGrandmaster => RatingGrandmasterIII,
            _ => RatingEliteGrandmaster
        };

    /// <summary>
    /// Gets a number representing the percentage of progress to the next sub-tier for the given rating
    /// </summary>
    /// <remarks>
    /// Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/>
    /// </remarks>
    public static double? GetNextTierFillPercentage(double rating)
    {
        var minRating = GetPreviousTierRating(rating);
        var maxRating = GetNextTierRating(rating);

        if (maxRating is null)
        {
            return null;
        }

        return (rating - minRating) / (maxRating - minRating);
    }

    /// <summary>
    /// Gets a percentage representing the progress to the next major tier for the given rating
    /// </summary>
    /// <remarks>
    /// Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/>
    /// </remarks>
    public static double? GetNextMajorTierFillPercentage(double rating)
    {
        var minRating = GetMajorTierRating(rating);
        var maxRating = GetNextMajorTierRating(rating);
        if (maxRating is null)
        {
            return null;
        }

        return (rating - minRating) / (maxRating.Value - minRating);
    }

    /// <summary>
    /// Denotes the related rating should be considered provisional
    /// </summary>
    /// <remarks>
    /// A rating should be considered provisional if the player meets any of the following criteria:
    /// - Rating volatility greater than or equal to 200
    /// - Played in less than 9 matches
    /// - Played in less than 3 tournaments
    /// </remarks>
    /// <param name="volatility">Volatility of the rating</param>
    /// <param name="matchesPlayed">Number of matches played by the player</param>
    /// <param name="tournamentsPlayed">Number of tournaments played by the player</param>
    public static bool IsProvisional(double volatility, int matchesPlayed, int tournamentsPlayed) =>
        volatility >= 200.0 || matchesPlayed <= 8 || tournamentsPlayed <= 2;

    /// <summary>
    /// Denotes the given tier is Elite Grandmaster
    /// </summary>
    public static bool IsEliteGrandmaster(string tier) =>
        tier == GetTier(RatingEliteGrandmaster);

    /// <summary>
    /// Denotes the given tier is any Grandmaster tier
    /// </summary>
    public static bool IsGrandmaster(string tier) =>
        tier == GetTier(RatingGrandmasterIII) ||
        tier == GetTier(RatingGrandmasterII) ||
        tier == GetTier(RatingGrandmasterI);

    /// <summary>
    /// Denotes the given tier is any Master tier
    /// </summary>
    public static bool IsMaster(string tier) =>
        tier == GetTier(RatingMasterIII) ||
        tier == GetTier(RatingMasterII) ||
        tier == GetTier(RatingMasterI);

    /// <summary>
    /// Denotes the given tier is any Diamond tier
    /// </summary>
    public static bool IsDiamond(string tier) =>
        tier == GetTier(RatingDiamondIII) ||
        tier == GetTier(RatingDiamondII) ||
        tier == GetTier(RatingDiamondI);

    /// <summary>
    /// Denotes the given tier is any Emerald tier
    /// </summary>
    public static bool IsEmerald(string tier) =>
        tier == GetTier(RatingEmeraldIII) ||
        tier == GetTier(RatingEmeraldII) ||
        tier == GetTier(RatingEmeraldI);

    /// <summary>
    /// Denotes the given tier is any Platinum tier
    /// </summary>
    public static bool IsPlatinum(string tier) =>
        tier == GetTier(RatingPlatinumIII) ||
        tier == GetTier(RatingPlatinumII) ||
        tier == GetTier(RatingPlatinumI);

    /// <summary>
    /// Denotes the given tier is any Gold tier
    /// </summary>
    public static bool IsGold(string tier) =>
        tier == GetTier(RatingGoldIII) ||
        tier == GetTier(RatingGoldII) ||
        tier == GetTier(RatingGoldI);

    /// <summary>
    /// Denotes the given tier is any Silver tier
    /// </summary>
    public static bool IsSilver(string tier) =>
        tier == GetTier(RatingSilverIII) ||
        tier == GetTier(RatingSilverII) ||
        tier == GetTier(RatingSilverI);

    /// <summary>
    /// Denotes the given tier is any Bronze tier
    /// </summary>
    public static bool IsBronze(string tier) =>
        tier == GetTier(RatingBronzeIII) ||
        tier == GetTier(RatingBronzeII) ||
        tier == GetTier(RatingBronzeI);
}
