using Database.Entities;
using Database.Entities.Processor;

namespace APITests.SeedData;

public static class SeededMatchRatingStats
{
    public static RatingAdjustment Get() =>
        new()
        {
            Id = 89,
            PlayerId = 11587,
            MatchId = 34762,
            RatingBefore = 1875,
            RatingAfter = 1840.8605516758596,
            RatingDelta = -34.1394483241404,
            VolatilityBefore = 187.5,
            VolatilityAfter = 186.47017393890098,
            VolatilityDelta = -1.0298260610990155,
            GlobalRankBefore = 8695,
            GlobalRankAfter = 8700,
            GlobalRankDelta = 5,
            CountryRankBefore = 104,
            CountryRankAfter = 104,
            CountryRankDelta = 0,
            PercentileBefore = 0.000689655172413793,
            PercentileAfter = 0.00011494252873563218,
            PercentileDelta = -0.0005747126436781609,
        };
}
