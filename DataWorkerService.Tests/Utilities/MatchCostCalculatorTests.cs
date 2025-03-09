using System.Diagnostics.CodeAnalysis;
using Common.Enums.Enums.Verification;
using Database.Entities;
using DataWorkerService.Utilities;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.Utilities;

public class MatchCostCalculatorTests
{
    [Fact]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public void OtrMatchCostFormula_ProducesAccurateData()
    {
        // Arrange
        Match match = SeededMatch.ExampleMatch();

        match.VerificationStatus = VerificationStatus.Verified;
        foreach (Game game in match.Games)
        {
            game.VerificationStatus = VerificationStatus.Verified;
            foreach (GameScore score in game.Scores)
            {
                score.VerificationStatus = VerificationStatus.Verified;
                score.ProcessingStatus = ScoreProcessingStatus.Done;
            }
        }

        var expected = new Dictionary<string, double>
        {
            ["CutPaper"] = 1.715632262,
            ["Zylice"] = 1.435673415,
            ["fieryrage"] = 1.412030894,
            ["xootynator"] = 1.406094682,
            ["hydrogen bomb"] = 1.399356596,
            ["tekkito"] = 1.342938389,
            ["Vaxei"] = 1.118492251,
            ["Ryuk"] = 1.057176372,
            ["Yip"] = 1.04709621,
            ["wudci"] = 0.9109595245,
            ["BoshyMan741"] = 0.8785213715,
            ["WindowLife"] = 0.7966767708,
            ["Kama"] = 0.7944238726,
            ["Vespirit"] = 0.7820572677,
            ["Stoof"] = 0.7409838806,
            ["kurtis-"] = 0.6382098891
        };

        // Act
        IDictionary<int, double> costs = MatchCostCalculator.CalculateOtrMatchCosts(match.Games);
        var mapped = match.Games
            .SelectMany(g => g.Scores)
            .Select(s => s.Player)
            .DistinctBy(p => p.Id)
            .Select(p => new { Player = p.Username, Cost = costs[p.Id] })
            .OrderByDescending(group => group.Cost)
            .ToList();

        // Assert
        Assert.All(mapped, map =>
        {
            Assert.True(Math.Abs(expected[map.Player] - map.Cost) < 0.00001);
        });
    }
}
