using API.Entities;

namespace APITests.SeedData;

public static class SeededMatch
{
    private static readonly Random s_rand = new();

    public static ICollection<Match> Generate(int tournamentId, int amount)
    {
        var matches = new List<Match>();

        for (var i = 0; i < amount; i++)
        {
            matches.Add(Generate(tournamentId, null));
        }

        return matches;
    }

    public static ICollection<Match> Generate(IEnumerable<long> osuIds)
    {
        var ids = osuIds.ToList();
        var matches = new List<Match>();

        for (var i = 0; i < ids.Count; i++)
        {
            matches.Add(Generate(ids.ElementAt(i)));
        }

        return matches;
    }

    public static ICollection<Match> Generate(IEnumerable<int> matchIds)
    {
        var ids = matchIds.ToList();
        var matches = new List<Match>();

        for (var i = 0; i < ids.Count; i++)
        {
            matches.Add(Generate(null, ids.ElementAt(i)));
        }

        return matches;
    }

    private static Match Generate(int? tournamentId, int? matchId)
    {
        var match = new Match
        {
            Id = s_rand.Next(),
            MatchId = matchId ?? s_rand.NextInt64(),
            Name = "OWC2021: (United States) vs (Germany)",
            StartTime = new DateTime(2022, 01, 23),
            Created = new DateTime(2023, 09, 30),
            Updated = new DateTime(2023, 11, 04),
            EndTime = new DateTime(2022, 01, 23),
            VerificationInfo = null,
            VerificationSource = 0,
            VerificationStatus = 0,
            SubmitterUserId = 21,
            VerifierUserId = null,
            TournamentId = tournamentId ?? 123,
            NeedsAutoCheck = false,
            IsApiProcessed = true
        };

        match.Games = SeededGame.Generate(match.Id, 5);
        return match;
    }

    private static Match Generate(long matchId)
    {
        var match = new Match
        {
            Id = s_rand.Next(),
            MatchId = matchId,
            Name = "OWC2021: (United States) vs (Germany)",
            StartTime = new DateTime(2022, 01, 23),
            Created = new DateTime(2023, 09, 30),
            Updated = new DateTime(2023, 11, 04),
            EndTime = new DateTime(2022, 01, 23),
            VerificationInfo = null,
            VerificationSource = 0,
            VerificationStatus = 0,
            SubmitterUserId = 21,
            VerifierUserId = null,
            TournamentId = 123,
            NeedsAutoCheck = false,
            IsApiProcessed = true
        };

        match.Games = SeededGame.Generate(match.Id, 5);
        return match;
    }
}
