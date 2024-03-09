using API.Entities;

namespace APITests.SeedData;

public static class SeededMatchScore
{
    private static readonly Random s_rand = new();
    private const int MAX_COMBO_LIMIT = 1200;
    private const int COUNT_50_LIMIT = 3;
    private const int COUNT_100_LIMIT = 50;
    private const int COUNT_300_LIMIT = (COUNT_50_LIMIT + COUNT_100_LIMIT + MAX_COMBO_LIMIT) * 2;
    private const int MISS_LIMIT = 10;

    public static IEnumerable<MatchScore> GetScoresForGame(
        int gameId,
        int? amountBlue = null,
        int? amountRed = null,
        int? amountHeadToHead = null
    )
    {
        var ret = new List<MatchScore>();

        if (amountBlue.HasValue)
        {
            for (var i = 0; i < amountBlue; i++)
            {
                ret.Add(Generate(gameId, 1));
            }
        }

        if (amountRed != null)
        {
            for (var i = 0; i < amountBlue; i++)
            {
                ret.Add(Generate(gameId, 2));
            }
        }

        if (amountHeadToHead != null)
        {
            for (var i = 0; i < amountBlue; i++)
            {
                ret.Add(Generate(gameId, 0));
            }
        }

        return ret;
    }

    private static MatchScore Generate(int gameId, int team)
    {
        var misses = s_rand.Next() % MISS_LIMIT;
        var perfect = misses == 0;

        return new MatchScore
        {
            Id = s_rand.Next(),
            GameId = gameId,
            Team = team,
            Score = s_rand.NextInt64(),
            MaxCombo = s_rand.Next() % 2000,
            Count50 = s_rand.Next() % COUNT_50_LIMIT,
            Count100 = s_rand.Next() % COUNT_100_LIMIT,
            Count300 = s_rand.Next() % COUNT_300_LIMIT,
            CountKatu = s_rand.Next() % (COUNT_100_LIMIT / 7),
            CountGeki = s_rand.Next() % (COUNT_300_LIMIT / 7),
            CountMiss = misses,
            Perfect = perfect,
            PlayerId = s_rand.Next() % 10000,
            IsValid = true,
            EnabledMods = null
        };
    }
}
