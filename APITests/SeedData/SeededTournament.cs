using API.Entities;

namespace APITests.SeedData;

public static class SeededTournament
{
	public static Tournament Generate()
	{
		var tournament = new Tournament
		{
			Id = 23,
			Name = "osu! World Cup 2021",
			Abbreviation = "OWC2021",
			ForumUrl = "https://osu.ppy.sh/wiki/en/Tournaments/OWC/2021",
			RankRangeLowerBound = 1,
			Mode = 0,
			TeamSize = 4,
			Created = new DateTime(2023, 10, 14),
			Updated = null
		};

		tournament.Matches = SeededMatch.Generate(tournament.Id, 5);
		return tournament;
	}
}