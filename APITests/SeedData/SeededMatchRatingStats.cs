using API.Entities;

namespace APITests.SeedData;

public static class SeededMatchRatingStats
{
	public static MatchRatingStats Get() => new()
	{
		Id = 1234,
		PlayerId = 541477,
		MatchId = 1983,
		MatchCost = 2.4,
		RatingBefore = 1500.2,
		RatingAfter = 1552.5,
		RatingChange = 52.3,
		VolatilityBefore = 4.2,
		VolatilityAfter = 3.5,
		VolatilityChange = -0.7,
		GlobalRankBefore = 50,
		GlobalRankAfter = 45,
		GlobalRankChange = 5,
		CountryRankBefore = 15,
		CountryRankAfter = 14,
		CountryRankChange = 1,
		PercentileBefore = 92.5,
		PercentileAfter = 94.3,
		PercentileChange = 1.8,
		AverageTeammateRating = 1542.6,
		AverageOpponentRating = 1572.8,
	};
}