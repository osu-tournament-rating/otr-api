using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;

namespace APITests.SeedData;

public static class SeededMatch
{
    private static readonly Random s_rand = new();

    public static ICollection<Match> Generate(int tournamentId, int amount)
    {
        var matches = new List<Match>();

        for (var i = 0; i < amount; i++)
        {
            matches.Add(Generate(tournamentId));
        }

        return matches;
    }

    private static Match Generate(int tournamentId)
    {
        var match = new Match
        {
            Id = s_rand.Next(),
            OsuId = s_rand.NextInt64(),
            Name = "OWC2021: (United States) vs (Germany)",
            StartTime = new DateTime(2022, 01, 23),
            Created = new DateTime(2023, 09, 30),
            Updated = new DateTime(2023, 11, 04),
            EndTime = new DateTime(2022, 01, 23),
            VerificationStatus = VerificationStatus.Verified,
            SubmittedByUserId = 21,
            VerifiedByUserId = null,
            TournamentId = tournamentId
        };

        match.Games = SeededGame.Generate(match.Id, 5);
        return match;
    }
}
