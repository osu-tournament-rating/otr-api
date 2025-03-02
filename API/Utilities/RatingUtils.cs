using Common.Rating;

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

    /// <summary>
    /// Gets the string representation of the given rating
    /// </summary>
    public static string GetTier(double rating) =>
        rating switch
        {
            < RatingConstants.RatingBronzeIII => "Bronze III",
            < RatingConstants.RatingBronzeI => "Bronze II",
            < RatingConstants.RatingSilverIII => "Bronze I",
            < RatingConstants.RatingSilverII => "Silver III",
            < RatingConstants.RatingSilverI => "Silver II",
            < RatingConstants.RatingGoldIII => "Silver I",
            < RatingConstants.RatingGoldII => "Gold III",
            < RatingConstants.RatingGoldI => "Gold II",
            < RatingConstants.RatingPlatinumIII => "Gold I",
            < RatingConstants.RatingPlatinumII => "Platinum III",
            < RatingConstants.RatingPlatinumI => "Platinum II",
            < RatingConstants.RatingEmeraldIII => "Platinum I",
            < RatingConstants.RatingEmeraldII => "Emerald III",
            < RatingConstants.RatingEmeraldI => "Emerald II",
            < RatingConstants.RatingDiamondIII => "Emerald I",
            < RatingConstants.RatingDiamondII => "Diamond III",
            < RatingConstants.RatingDiamondI => "Diamond II",
            < RatingConstants.RatingMasterIII => "Diamond I",
            < RatingConstants.RatingMasterII => "Master III",
            < RatingConstants.RatingMasterI => "Master II",
            < RatingConstants.RatingGrandmasterIII => "Master I",
            < RatingConstants.RatingGrandmasterII => "Grandmaster III",
            < RatingConstants.RatingGrandmasterI => "Grandmaster II",
            < RatingConstants.RatingEliteGrandmaster => "Grandmaster I",
            _ => "Elite Grandmaster"
        };

    /// <summary>
    /// Gets the integer representation of the sub-tier for the given rating
    /// </summary>
    /// <remarks>Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static int? GetSubTier(double rating) =>
        rating switch
        {
            < RatingConstants.RatingBronzeII => 3,
            < RatingConstants.RatingBronzeI => 2,
            < RatingConstants.RatingSilverIII => 1,
            < RatingConstants.RatingSilverII => 3,
            < RatingConstants.RatingSilverI => 2,
            < RatingConstants.RatingGoldIII => 1,
            < RatingConstants.RatingGoldII => 3,
            < RatingConstants.RatingGoldI => 2,
            < RatingConstants.RatingPlatinumIII => 1,
            < RatingConstants.RatingPlatinumII => 3,
            < RatingConstants.RatingPlatinumI => 2,
            < RatingConstants.RatingEmeraldIII => 1,
            < RatingConstants.RatingEmeraldII => 3,
            < RatingConstants.RatingEmeraldI => 2,
            < RatingConstants.RatingDiamondIII => 1,
            < RatingConstants.RatingDiamondII => 3,
            < RatingConstants.RatingDiamondI => 2,
            < RatingConstants.RatingMasterIII => 1,
            < RatingConstants.RatingMasterII => 3,
            < RatingConstants.RatingMasterI => 2,
            < RatingConstants.RatingGrandmasterIII => 1,
            < RatingConstants.RatingGrandmasterII => 3,
            < RatingConstants.RatingGrandmasterI => 2,
            < RatingConstants.RatingEliteGrandmaster => 1,
            _ => null
        };

    /// <summary>
    /// Gets the string representation of the next tier for the given rating
    /// </summary>
    /// <remarks>Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static string? GetNextTier(double rating) =>
        rating switch
        {
            < RatingConstants.RatingBronzeII => GetTier(RatingConstants.RatingBronzeII),
            < RatingConstants.RatingBronzeI => GetTier(RatingConstants.RatingBronzeI),
            < RatingConstants.RatingSilverIII => GetTier(RatingConstants.RatingSilverIII),
            < RatingConstants.RatingSilverII => GetTier(RatingConstants.RatingSilverII),
            < RatingConstants.RatingSilverI => GetTier(RatingConstants.RatingSilverI),
            < RatingConstants.RatingGoldIII => GetTier(RatingConstants.RatingGoldIII),
            < RatingConstants.RatingGoldII => GetTier(RatingConstants.RatingGoldII),
            < RatingConstants.RatingGoldI => GetTier(RatingConstants.RatingGoldI),
            < RatingConstants.RatingPlatinumIII => GetTier(RatingConstants.RatingPlatinumIII),
            < RatingConstants.RatingPlatinumII => GetTier(RatingConstants.RatingPlatinumII),
            < RatingConstants.RatingPlatinumI => GetTier(RatingConstants.RatingPlatinumI),
            < RatingConstants.RatingEmeraldIII => GetTier(RatingConstants.RatingEmeraldIII),
            < RatingConstants.RatingEmeraldII => GetTier(RatingConstants.RatingEmeraldII),
            < RatingConstants.RatingEmeraldI => GetTier(RatingConstants.RatingEmeraldI),
            < RatingConstants.RatingDiamondIII => GetTier(RatingConstants.RatingDiamondIII),
            < RatingConstants.RatingDiamondII => GetTier(RatingConstants.RatingDiamondII),
            < RatingConstants.RatingDiamondI => GetTier(RatingConstants.RatingDiamondI),
            < RatingConstants.RatingMasterIII => GetTier(RatingConstants.RatingMasterIII),
            < RatingConstants.RatingMasterII => GetTier(RatingConstants.RatingMasterII),
            < RatingConstants.RatingMasterI => GetTier(RatingConstants.RatingMasterI),
            < RatingConstants.RatingGrandmasterIII => GetTier(RatingConstants.RatingGrandmasterIII),
            < RatingConstants.RatingGrandmasterII => GetTier(RatingConstants.RatingGrandmasterII),
            < RatingConstants.RatingGrandmasterI => GetTier(RatingConstants.RatingGrandmasterI),
            < RatingConstants.RatingEliteGrandmaster => GetTier(RatingConstants.RatingEliteGrandmaster),
            _ => null
        };

    /// <summary>
    /// Gets the rating threshold of the next tier for the given rating
    /// </summary>
    /// <remarks>Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static double? GetNextTierRating(double rating) =>
        rating switch
        {
            < RatingConstants.RatingBronzeII => RatingConstants.RatingBronzeII,
            < RatingConstants.RatingBronzeI => RatingConstants.RatingBronzeI,
            < RatingConstants.RatingSilverIII => RatingConstants.RatingSilverIII,
            < RatingConstants.RatingSilverII => RatingConstants.RatingSilverII,
            < RatingConstants.RatingSilverI => RatingConstants.RatingSilverI,
            < RatingConstants.RatingGoldIII => RatingConstants.RatingGoldIII,
            < RatingConstants.RatingGoldII => RatingConstants.RatingGoldII,
            < RatingConstants.RatingGoldI => RatingConstants.RatingGoldI,
            < RatingConstants.RatingPlatinumIII => RatingConstants.RatingPlatinumIII,
            < RatingConstants.RatingPlatinumII => RatingConstants.RatingPlatinumII,
            < RatingConstants.RatingPlatinumI => RatingConstants.RatingPlatinumI,
            < RatingConstants.RatingEmeraldIII => RatingConstants.RatingEmeraldIII,
            < RatingConstants.RatingEmeraldII => RatingConstants.RatingEmeraldII,
            < RatingConstants.RatingEmeraldI => RatingConstants.RatingEmeraldI,
            < RatingConstants.RatingDiamondIII => RatingConstants.RatingDiamondIII,
            < RatingConstants.RatingDiamondII => RatingConstants.RatingDiamondII,
            < RatingConstants.RatingDiamondI => RatingConstants.RatingDiamondI,
            < RatingConstants.RatingMasterIII => RatingConstants.RatingMasterIII,
            < RatingConstants.RatingMasterII => RatingConstants.RatingMasterII,
            < RatingConstants.RatingMasterI => RatingConstants.RatingMasterI,
            < RatingConstants.RatingGrandmasterIII => RatingConstants.RatingGrandmasterIII,
            < RatingConstants.RatingGrandmasterII => RatingConstants.RatingGrandmasterII,
            < RatingConstants.RatingGrandmasterI => RatingConstants.RatingGrandmasterI,
            < RatingConstants.RatingEliteGrandmaster => RatingConstants.RatingEliteGrandmaster,
            _ => null
        };

    /// <summary>
    /// Gets the difference in rating of the next tier for the given rating
    /// </summary>
    /// <remarks>Will return 0 for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static double GetNextTierRatingDelta(double rating) =>
        rating switch
        {
            < RatingConstants.RatingBronzeII => RatingConstants.RatingBronzeII - rating,
            < RatingConstants.RatingBronzeI => RatingConstants.RatingBronzeI - rating,
            < RatingConstants.RatingSilverIII => RatingConstants.RatingSilverIII - rating,
            < RatingConstants.RatingSilverII => RatingConstants.RatingSilverII - rating,
            < RatingConstants.RatingSilverI => RatingConstants.RatingSilverI - rating,
            < RatingConstants.RatingGoldIII => RatingConstants.RatingGoldIII - rating,
            < RatingConstants.RatingGoldII => RatingConstants.RatingGoldII - rating,
            < RatingConstants.RatingGoldI => RatingConstants.RatingGoldI - rating,
            < RatingConstants.RatingPlatinumIII => RatingConstants.RatingPlatinumIII - rating,
            < RatingConstants.RatingPlatinumII => RatingConstants.RatingPlatinumII - rating,
            < RatingConstants.RatingPlatinumI => RatingConstants.RatingPlatinumI - rating,
            < RatingConstants.RatingEmeraldIII => RatingConstants.RatingEmeraldIII - rating,
            < RatingConstants.RatingEmeraldII => RatingConstants.RatingEmeraldII - rating,
            < RatingConstants.RatingEmeraldI => RatingConstants.RatingEmeraldI - rating,
            < RatingConstants.RatingDiamondIII => RatingConstants.RatingDiamondIII - rating,
            < RatingConstants.RatingDiamondII => RatingConstants.RatingDiamondII - rating,
            < RatingConstants.RatingDiamondI => RatingConstants.RatingDiamondI - rating,
            < RatingConstants.RatingMasterIII => RatingConstants.RatingMasterIII - rating,
            < RatingConstants.RatingMasterII => RatingConstants.RatingMasterII - rating,
            < RatingConstants.RatingMasterI => RatingConstants.RatingMasterI - rating,
            < RatingConstants.RatingGrandmasterIII => RatingConstants.RatingGrandmasterIII - rating,
            < RatingConstants.RatingGrandmasterII => RatingConstants.RatingGrandmasterII - rating,
            < RatingConstants.RatingGrandmasterI => RatingConstants.RatingGrandmasterI - rating,
            < RatingConstants.RatingEliteGrandmaster => RatingConstants.RatingEliteGrandmaster - rating,
            _ => 0
        };

    /// <summary>
    /// Gets the rating threshold of the previous tier for the given rating
    /// </summary>
    public static double GetPreviousTierRating(double rating) =>
        rating switch
        {
            < RatingConstants.RatingBronzeII => RatingConstants.RatingBronzeIII,
            < RatingConstants.RatingBronzeI => RatingConstants.RatingBronzeII,
            < RatingConstants.RatingSilverIII => RatingConstants.RatingBronzeI,
            < RatingConstants.RatingSilverII => RatingConstants.RatingSilverIII,
            < RatingConstants.RatingSilverI => RatingConstants.RatingSilverII,
            < RatingConstants.RatingGoldIII => RatingConstants.RatingSilverI,
            < RatingConstants.RatingGoldII => RatingConstants.RatingGoldIII,
            < RatingConstants.RatingGoldI => RatingConstants.RatingGoldII,
            < RatingConstants.RatingPlatinumIII => RatingConstants.RatingGoldI,
            < RatingConstants.RatingPlatinumII => RatingConstants.RatingPlatinumIII,
            < RatingConstants.RatingPlatinumI => RatingConstants.RatingPlatinumII,
            < RatingConstants.RatingEmeraldIII => RatingConstants.RatingPlatinumI,
            < RatingConstants.RatingEmeraldII => RatingConstants.RatingEmeraldIII,
            < RatingConstants.RatingEmeraldI => RatingConstants.RatingEmeraldII,
            < RatingConstants.RatingDiamondIII => RatingConstants.RatingEmeraldI,
            < RatingConstants.RatingDiamondII => RatingConstants.RatingDiamondIII,
            < RatingConstants.RatingDiamondI => RatingConstants.RatingDiamondII,
            < RatingConstants.RatingMasterIII => RatingConstants.RatingDiamondI,
            < RatingConstants.RatingMasterII => RatingConstants.RatingMasterIII,
            < RatingConstants.RatingMasterI => RatingConstants.RatingMasterII,
            < RatingConstants.RatingGrandmasterIII => RatingConstants.RatingMasterI,
            < RatingConstants.RatingGrandmasterII => RatingConstants.RatingGrandmasterIII,
            < RatingConstants.RatingGrandmasterI => RatingConstants.RatingGrandmasterII,
            < RatingConstants.RatingEliteGrandmaster => RatingConstants.RatingGrandmasterI,
            _ => RatingConstants.RatingEliteGrandmaster
        };

    /// <summary>
    /// Gets the string representation of the next major tier for the given rating
    /// </summary>
    /// <remarks>Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static string? GetNextMajorTier(double rating) =>
        rating switch
        {
            < RatingConstants.RatingSilverIII => GetTier(RatingConstants.RatingSilverIII),
            < RatingConstants.RatingGoldIII => GetTier(RatingConstants.RatingGoldIII),
            < RatingConstants.RatingPlatinumIII => GetTier(RatingConstants.RatingPlatinumIII),
            < RatingConstants.RatingEmeraldIII => GetTier(RatingConstants.RatingEmeraldIII),
            < RatingConstants.RatingDiamondIII => GetTier(RatingConstants.RatingDiamondIII),
            < RatingConstants.RatingMasterIII => GetTier(RatingConstants.RatingMasterIII),
            < RatingConstants.RatingGrandmasterIII => GetTier(RatingConstants.RatingGrandmasterIII),
            < RatingConstants.RatingEliteGrandmaster => GetTier(RatingConstants.RatingEliteGrandmaster),
            _ => null
        };

    /// <summary>
    /// Gets the rating threshold of the next major tier for the given rating
    /// </summary>
    /// <remarks>Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static double? GetNextMajorTierRating(double rating) =>
        rating switch
        {
            < RatingConstants.RatingSilverIII => RatingConstants.RatingSilverIII,
            < RatingConstants.RatingGoldIII => RatingConstants.RatingGoldIII,
            < RatingConstants.RatingPlatinumIII => RatingConstants.RatingPlatinumIII,
            < RatingConstants.RatingEmeraldIII => RatingConstants.RatingEmeraldIII,
            < RatingConstants.RatingDiamondIII => RatingConstants.RatingDiamondIII,
            < RatingConstants.RatingMasterIII => RatingConstants.RatingMasterIII,
            < RatingConstants.RatingGrandmasterIII => RatingConstants.RatingGrandmasterIII,
            < RatingConstants.RatingEliteGrandmaster => RatingConstants.RatingEliteGrandmaster,
            _ => null
        };

    /// <summary>
    /// Gets the difference in rating of the next major tier for the given rating
    /// </summary>
    /// <remarks>Will return 0 for given ratings greater than <see cref="RatingEliteGrandmaster"/></remarks>
    public static double GetNextMajorTierRatingDelta(double rating) =>
        rating switch
        {
            < RatingConstants.RatingSilverIII => RatingConstants.RatingSilverIII - rating,
            < RatingConstants.RatingGoldIII => RatingConstants.RatingGoldIII - rating,
            < RatingConstants.RatingPlatinumIII => RatingConstants.RatingPlatinumIII - rating,
            < RatingConstants.RatingEmeraldIII => RatingConstants.RatingEmeraldIII - rating,
            < RatingConstants.RatingDiamondIII => RatingConstants.RatingDiamondIII - rating,
            < RatingConstants.RatingMasterIII => RatingConstants.RatingMasterIII - rating,
            < RatingConstants.RatingGrandmasterIII => RatingConstants.RatingGrandmasterIII - rating,
            < RatingConstants.RatingEliteGrandmaster => RatingConstants.RatingEliteGrandmaster - rating,
            _ => 0
        };

    /// <summary>
    /// Gets the rating threshold of the major tier for the given rating
    /// </summary>
    public static double GetMajorTierRating(double rating) =>
        rating switch
        {
            < RatingConstants.RatingSilverIII => RatingConstants.RatingBronzeIII,
            < RatingConstants.RatingGoldIII => RatingConstants.RatingSilverIII,
            < RatingConstants.RatingPlatinumIII => RatingConstants.RatingGoldIII,
            < RatingConstants.RatingEmeraldIII => RatingConstants.RatingPlatinumIII,
            < RatingConstants.RatingDiamondIII => RatingConstants.RatingEmeraldIII,
            < RatingConstants.RatingMasterIII => RatingConstants.RatingDiamondIII,
            < RatingConstants.RatingGrandmasterIII => RatingConstants.RatingMasterIII,
            < RatingConstants.RatingEliteGrandmaster => RatingConstants.RatingGrandmasterIII,
            _ => RatingConstants.RatingEliteGrandmaster
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
        tier == GetTier(RatingConstants.RatingEliteGrandmaster);

    /// <summary>
    /// Denotes the given tier is any Grandmaster tier
    /// </summary>
    public static bool IsGrandmaster(string tier) =>
        tier == GetTier(RatingConstants.RatingGrandmasterIII) ||
        tier == GetTier(RatingConstants.RatingGrandmasterII) ||
        tier == GetTier(RatingConstants.RatingGrandmasterI);

    /// <summary>
    /// Denotes the given tier is any Master tier
    /// </summary>
    public static bool IsMaster(string tier) =>
        tier == GetTier(RatingConstants.RatingMasterIII) ||
        tier == GetTier(RatingConstants.RatingMasterII) ||
        tier == GetTier(RatingConstants.RatingMasterI);

    /// <summary>
    /// Denotes the given tier is any Diamond tier
    /// </summary>
    public static bool IsDiamond(string tier) =>
        tier == GetTier(RatingConstants.RatingDiamondIII) ||
        tier == GetTier(RatingConstants.RatingDiamondII) ||
        tier == GetTier(RatingConstants.RatingDiamondI);

    /// <summary>
    /// Denotes the given tier is any Emerald tier
    /// </summary>
    public static bool IsEmerald(string tier) =>
        tier == GetTier(RatingConstants.RatingEmeraldIII) ||
        tier == GetTier(RatingConstants.RatingEmeraldII) ||
        tier == GetTier(RatingConstants.RatingEmeraldI);

    /// <summary>
    /// Denotes the given tier is any Platinum tier
    /// </summary>
    public static bool IsPlatinum(string tier) =>
        tier == GetTier(RatingConstants.RatingPlatinumIII) ||
        tier == GetTier(RatingConstants.RatingPlatinumII) ||
        tier == GetTier(RatingConstants.RatingPlatinumI);

    /// <summary>
    /// Denotes the given tier is any Gold tier
    /// </summary>
    public static bool IsGold(string tier) =>
        tier == GetTier(RatingConstants.RatingGoldIII) ||
        tier == GetTier(RatingConstants.RatingGoldII) ||
        tier == GetTier(RatingConstants.RatingGoldI);

    /// <summary>
    /// Denotes the given tier is any Silver tier
    /// </summary>
    public static bool IsSilver(string tier) =>
        tier == GetTier(RatingConstants.RatingSilverIII) ||
        tier == GetTier(RatingConstants.RatingSilverII) ||
        tier == GetTier(RatingConstants.RatingSilverI);

    /// <summary>
    /// Denotes the given tier is any Bronze tier
    /// </summary>
    public static bool IsBronze(string tier) =>
        tier == GetTier(RatingConstants.RatingBronzeIII) ||
        tier == GetTier(RatingConstants.RatingBronzeII) ||
        tier == GetTier(RatingConstants.RatingBronzeI);
}
