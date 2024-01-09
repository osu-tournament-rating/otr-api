using API.Entities;

namespace APITests.SeedData;

public static class SeededGame
{
	public static Game Get() => new()
	{
		Id = 198743,
		MatchId = 35214,
		BeatmapId = 24245,
		PlayMode = 0,
		ScoringType = 3,
		TeamType = 0,
		Mods = 0,
		PostModSr = 6.36389,
		GameId = 502333236,
		VerificationStatus = 2,
		RejectionReason = 0,
		Created = new DateTime(2023,09,14),
		StartTime = new DateTime(2023,03,10),
		EndTime = new DateTime(2023,03,10),
		Updated = new DateTime(2023,11,04),
	};
}