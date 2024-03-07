using API.Entities;

namespace APITests.SeedData;

public static class SeededPlayerMatchStats
{
    public static PlayerMatchStats Get() =>
        new()
        {
            Id = 74,
            PlayerId = 62,
            MatchId = 34762,
            Won = true,
            AverageScore = 430545,
            AverageMisses = 13,
            AverageAccuracy = 95.25029874826853,
            GamesPlayed = 2,
            AveragePlacement = 5,
            GamesWon = 2,
            GamesLost = 0,
            TeammateIds = [194, 1421, 11217],
            OpponentIds = [1046, 5482, 6070, 11067, 11068],
        };
}
