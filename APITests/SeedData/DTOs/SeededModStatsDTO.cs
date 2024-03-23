using API.DTOs;

namespace APITests.SeedData;

public class SeededModStatsDTO
{
    public static ModStatsDTO Get()
    {
        return new ModStatsDTO
        {
            GamesPlayed = 21,
            GamesWon = 19,
            Winrate = 21 / 19.0,
            NormalizedAverageScore = 500000
        };
    }
}
