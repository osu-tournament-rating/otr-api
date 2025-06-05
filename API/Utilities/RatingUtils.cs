using Common.Constants;

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

    public const int RatingMinimum = 100;

    /// <summary>
    /// Gets the major tier representation of the specified rating
    /// </summary>
    public static string GetMajorTier(double rating)
    {
        return rating switch
        {
            < RatingConstants.RatingSilverIII => "Bronze",
            < RatingConstants.RatingGoldIII => "Silver",
            < RatingConstants.RatingPlatinumIII => "Gold",
            < RatingConstants.RatingEmeraldIII => "Platinum",
            < RatingConstants.RatingDiamondIII => "Emerald",
            < RatingConstants.RatingMasterIII => "Diamond",
            < RatingConstants.RatingGrandmasterIII => "Master",
            < RatingConstants.RatingEliteGrandmaster => "Grandmaster",
            _ => "Elite Grandmaster"
        };
    }

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
    /// <remarks>
    /// Will return null for given ratings greater than <see cref="RatingEliteGrandmaster"/>
    /// </remarks>
    public static string? GetNextMajorTier(double rating) =>
        rating switch
        {
            < RatingConstants.RatingSilverIII => GetMajorTier(RatingConstants.RatingSilverIII),
            < RatingConstants.RatingGoldIII => GetMajorTier(RatingConstants.RatingGoldIII),
            < RatingConstants.RatingPlatinumIII => GetMajorTier(RatingConstants.RatingPlatinumIII),
            < RatingConstants.RatingEmeraldIII => GetMajorTier(RatingConstants.RatingEmeraldIII),
            < RatingConstants.RatingDiamondIII => GetMajorTier(RatingConstants.RatingDiamondIII),
            < RatingConstants.RatingMasterIII => GetMajorTier(RatingConstants.RatingMasterIII),
            < RatingConstants.RatingGrandmasterIII => GetMajorTier(RatingConstants.RatingGrandmasterIII),
            < RatingConstants.RatingEliteGrandmaster => GetMajorTier(RatingConstants.RatingEliteGrandmaster),
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
        double minRating = GetPreviousTierRating(rating);
        double? maxRating = GetNextTierRating(rating);

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
        double minRating = GetMajorTierRating(rating);
        double? maxRating = GetNextMajorTierRating(rating);
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
    /// Gets the integer representation of the next sub-tier for the given rating
    /// </summary>
    /// <remarks>Will return null for given ratings at or above <see cref="RatingGrandmasterI"/></remarks>
    public static int? GetNextSubTier(double rating)
    {
        // First get the current sub-tier (1, 2, or 3) for the given rating
        // This will be null for Elite Grandmaster
        int? currentSubTier = GetSubTier(rating);

        switch (currentSubTier)
        {
            // If current sub-tier is null (Elite Grandmaster), there is no next tier
            case null:
                return null;

            // If current sub-tier is 2 or 3, the next sub-tier is simply one less
            // (Sub-tiers count down: 3 → 2 → 1 within each major tier)
            // Example: Bronze III (3) → Bronze II (2) → Bronze I (1)
            case > 1:
                return currentSubTier - 1;

            // If current sub-tier is 1, we need to move to the next major tier
            default:
                {
                    // Check if there's a next major tier available
                    string? nextMajorTier = GetNextMajorTier(rating);

                    // Elite Grandmaster is the highest tier, so there's no next tier after it.
                    // Therefore, there is no sub tier and we return null.
                    if (nextMajorTier == "Elite Grandmaster")
                    {
                        return null;
                    }

                    // If there is a next major tier, return 3 (the highest sub-tier of that major tier)
                    // Example: Bronze I (1) → Silver III (3)
                    // If there's no next major tier, return null
                    return nextMajorTier != null ? 3 : null;
                }
        }
    }
}
