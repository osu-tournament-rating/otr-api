using API.Entities;
using Newtonsoft.Json;

namespace APITests.Osu;

public class MatchScoreTests
{
    [Theory]
    [InlineData(1100, 22, 0, 0, 98.69)]
    [InlineData(899, 25, 8, 21, 95.35)]
    public void StandardAccuracy_Computation_IsCorrect(
        int threeHundreds,
        int oneHundreds,
        int fifties,
        int misses,
        double expectedAccuracy
    )
    {
        var matchScore = new MatchScore
        {
            Count50 = fifties,
            Count100 = oneHundreds,
            Count300 = threeHundreds,
            CountMiss = misses
        };

        Assert.True(Math.Abs(expectedAccuracy - matchScore.AccuracyStandard) < 0.01);
    }

    [Theory]
    [InlineData(2990, 157, 0, 0, 0, 0, 99.92)]
    [InlineData(2539, 237, 0, 2, 0, 2, 99.74)]
    public void ManiaAccuracy_Computation_IsCorrect(
        int max,
        int threeHundreds,
        int twoHundreds,
        int oneHundreds,
        int fifties,
        int misses,
        double expectedAccuracy
    )
    {
        var matchScore = new MatchScore
        {
            CountGeki = max,
            Count300 = threeHundreds,
            CountKatu = twoHundreds,
            Count100 = oneHundreds,
            Count50 = fifties,
            CountMiss = misses
        };

        Assert.InRange(matchScore.AccuracyMania, expectedAccuracy - 0.01, expectedAccuracy + 0.01);
    }

    [Theory]
    [InlineData(1563, 62, 0, 9, 97.55)]
    [InlineData(1814, 70, 0, 0, 98.14)]
    public void TaikoAccuracy_Computation_IsCorrect(
        int threeHundreds,
        int oneHundreds,
        int fifties,
        int misses,
        double expectedAccuracy
    )
    {
        var matchScore = new MatchScore
        {
            Count50 = fifties,
            Count100 = oneHundreds,
            Count300 = threeHundreds,
            CountMiss = misses
        };

        Assert.InRange(matchScore.AccuracyTaiko, expectedAccuracy - 0.01, expectedAccuracy + 0.01);
    }

    [Theory]
    [InlineData(1522, 4, 164, 21, 6, 98.43)]
    public void CatchAccuracy_Computation_IsCorrect(
        int threeHundreds,
        int oneHundreds,
        int fifties,
        int katu,
        int misses,
        double expectedAccuracy
    )
    {
        var matchScore = new MatchScore
        {
            Count50 = fifties,
            Count100 = oneHundreds,
            Count300 = threeHundreds,
            CountKatu = katu,
            CountMiss = misses
        };

        Assert.InRange(matchScore.AccuracyCatch, expectedAccuracy - 0.01, expectedAccuracy + 0.01);
    }

    [Fact]
    public void Accuracy_IncludedInJsonSerialization()
    {
        var matchScore = new MatchScore() { Count300 = 500 };

        string json = JsonConvert.SerializeObject(matchScore);
        Assert.Contains("Accuracy", json);
        Assert.Contains("100.0", json);
    }

    [Fact]
    public void Accuracy_NeverReturnsNaN()
    {
        var matchScore = new MatchScore
        {
            Count300 = 0,
            Count100 = 0,
            Count50 = 0,
            CountMiss = 0
        };

        Assert.Equal(0, matchScore.AccuracyStandard);
        Assert.Equal(0, matchScore.AccuracyTaiko);
        Assert.Equal(0, matchScore.AccuracyCatch);
        Assert.Equal(0, matchScore.AccuracyMania);
    }
}
