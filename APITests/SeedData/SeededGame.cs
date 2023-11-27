using API.Entities;

namespace APITests.SeedData;

public static class SeededGame
{
	public static Game Get() => new()
	{
		Id = 12345789,
		MatchId = 123456,
		BeatmapId = 35353,
		PlayMode = 0,
		ScoringType = 3,
		TeamType = 2,
		Mods = 1025,
		PostModSr = 6.385,
		GameId = 554591768,
		VerificationStatus = 2,
		RejectionReason = 0,
		Created = new DateTime(2023,11,12),
		StartTime = new DateTime(2023,10,12),
		EndTime = new DateTime(2023,10,12),
		Updated = new DateTime(2023,11,12),
	};
}