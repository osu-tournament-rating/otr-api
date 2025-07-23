
using Common.Enums;
using Common.Enums.Verification;
using DWS.AutomationChecks;
using Microsoft.Extensions.Logging;
using Moq;
using TestingUtils.SeededData;

namespace DWS.Tests.AutomationChecks;

public class ScoreAutomationChecksTests
{
    private readonly Mock<ILogger<ScoreAutomationChecks>> _loggerMock = new();
    private readonly ScoreAutomationChecks _checker;

    public ScoreAutomationChecksTests()
    {
        _checker = new ScoreAutomationChecks(_loggerMock.Object);
    }

    [Theory]
    [InlineData(1001, ScoreRejectionReason.None)]
    [InlineData(1000, ScoreRejectionReason.ScoreBelowMinimum)]
    [InlineData(0, ScoreRejectionReason.ScoreBelowMinimum)]
    public void Process_ReturnsCorrectRejectionReason_ForScoreMinimum(long scoreAmount, ScoreRejectionReason expected)
    {
        // Arrange
        var tournament = SeededTournament.Generate(ruleset: Ruleset.Osu);
        var match = SeededMatch.Generate(tournament: tournament);
        var game = SeededGame.Generate(match: match, ruleset: Ruleset.Osu);
        var score = SeededScore.Generate(score: (int)scoreAmount, ruleset: Ruleset.Osu, mods: Mods.None, game: game);

        // Act
        ScoreRejectionReason result = _checker.Process(score);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Mods.None, ScoreRejectionReason.None)]
    [InlineData(Mods.Hidden, ScoreRejectionReason.None)]
    [InlineData(Mods.SuddenDeath, ScoreRejectionReason.InvalidMods)]
    [InlineData(Mods.Perfect, ScoreRejectionReason.InvalidMods)]
    [InlineData(Mods.Relax, ScoreRejectionReason.InvalidMods)]
    [InlineData(Mods.Autoplay, ScoreRejectionReason.InvalidMods)]
    [InlineData(Mods.Relax2, ScoreRejectionReason.InvalidMods)]
    [InlineData(Mods.Hidden | Mods.SuddenDeath, ScoreRejectionReason.InvalidMods)]
    public void Process_ReturnsCorrectRejectionReason_ForMods(Mods mods, ScoreRejectionReason expected)
    {
        // Arrange
        var tournament = SeededTournament.Generate(ruleset: Ruleset.Osu);
        var match = SeededMatch.Generate(tournament: tournament);
        var game = SeededGame.Generate(match: match, ruleset: Ruleset.Osu);
        var score = SeededScore.Generate(score: 50000, mods: mods, ruleset: Ruleset.Osu, game: game);

        // Act
        ScoreRejectionReason result = _checker.Process(score);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Ruleset.Osu, Ruleset.Osu, ScoreRejectionReason.None)]
    [InlineData(Ruleset.Taiko, Ruleset.Taiko, ScoreRejectionReason.None)]
    [InlineData(Ruleset.Osu, Ruleset.Taiko, ScoreRejectionReason.RulesetMismatch)]
    [InlineData(Ruleset.Taiko, Ruleset.Osu, ScoreRejectionReason.RulesetMismatch)]
    public void Process_ReturnsCorrectRejectionReason_ForRuleset(Ruleset scoreRuleset, Ruleset tournamentRuleset, ScoreRejectionReason expected)
    {
        // Arrange
        var tournament = SeededTournament.Generate(ruleset: tournamentRuleset);
        var match = SeededMatch.Generate(tournament: tournament);
        var game = SeededGame.Generate(match: match, ruleset: scoreRuleset);
        var score = SeededScore.Generate(score: 50000, ruleset: scoreRuleset, mods: Mods.None, game: game);

        // Act
        ScoreRejectionReason result = _checker.Process(score);

        // Assert
        Assert.Equal(expected, result);
    }
}
