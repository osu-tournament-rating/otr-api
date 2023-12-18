using API.Entities;

namespace APITests.SeedData;

public static class SeededMatch
{
	public static Match Get() => new()
	{
		Id = 40791,
		MatchId = 96880393,
		Name = "BQT4: (TeamDNA) vs (Down Under)",
		StartTime = new DateTime(2022,01,23),
		Created = new DateTime(2023,09,30),
		Updated = new DateTime(2023,11,04),
		EndTime = new DateTime(2022,01,23),
		VerificationInfo = null,
		VerificationSource = 0,
		VerificationStatus = 0,
		SubmitterUserId = 21,
		VerifierUserId = null,
		TournamentId = 152,
		NeedsAutoCheck = false,
		IsApiProcessed = true,
	};
}