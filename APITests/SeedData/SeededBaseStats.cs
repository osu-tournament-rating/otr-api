using API.Entities;

namespace APITests.SeedData;

public static class SeededBaseStats
{
	public static BaseStats Get() => new()
	{
		Id = 1,
		PlayerId = 1,
		Mode = 0,
		Rating = 1245.324,
		Volatility = 100.5231,
		Percentile = 0.3431,
		GlobalRank = 20,
		CountryRank = 2,
		MatchCostAverage = 1.23424,
		Created = new DateTime(2023, 11, 11),
		Updated = new DateTime(2023, 11, 12)
	};
}