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
            VolatilityBefore = 187.5,
            VolatilityAfter = 186.47017393890098,
        };
}
