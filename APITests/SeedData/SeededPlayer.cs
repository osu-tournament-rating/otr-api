using API.Entities;

namespace APITests.SeedData;

public static class SeededPlayer
{
	public static Player Get() => new()
	{
		Id = 334,
		OsuId = 4787150,
		Created = new DateTime(2023, 08, 12),
		RankStandard = 26,
		RankTaiko = 2603684,
		RankCatch = 2226949,
		RankMania = 293056,
		Updated = new DateTime(2023,09,17),
		Username = "Vaxei",
		Country = "US",
		EarliestCatchGlobalRank = 2226949,
		EarliestCatchGlobalRankDate = new DateTime(2023,09,18),
		EarliestManiaGlobalRank = 293056,
		EarliestManiaGlobalRankDate = new DateTime(2023,09,18),
		EarliestOsuGlobalRank = 8,
		EarliestOsuGlobalRankDate = new DateTime(2018,11,10),
		EarliestTaikoGlobalRank = 2603684,
		EarliestTaikoGlobalRankDate = new DateTime(2023,09,18),
	};
}