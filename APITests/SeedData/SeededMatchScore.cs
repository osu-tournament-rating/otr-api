using API.Entities;

namespace APITests.SeedData;

public static class SeededMatchScore
{
	public static MatchScore Get() => new()
	{
		Id = 749311,
		GameId = 274393,
		Team = 1,
		Score = 700810,
		MaxCombo = 426,
		Count50 = 0,
		Count100 = 21,
		Count300 = 464,
		CountMiss = 8,
		Perfect = false,
		Pass = true,
		EnabledMods = 0,
		CountKatu = 8,
		CountGeki = 114,
		PlayerId = 4962,
		IsValid = true,
	};
}