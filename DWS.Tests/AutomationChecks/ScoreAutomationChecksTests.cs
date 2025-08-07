
using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.AutomationChecks;
using Microsoft.Extensions.Logging;
using Moq;
using TestingUtils.SeededData;
using Match = Database.Entities.Match;

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
        Tournament tournament = SeededTournament.Generate(ruleset: Ruleset.Osu);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: Ruleset.Osu);
        GameScore score = SeededScore.Generate(score: (int)scoreAmount, ruleset: Ruleset.Osu, mods: Mods.None, game: game);

        // Act
        ScoreRejectionReason result = _checker.Process(score, tournament.Ruleset);

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
        Tournament tournament = SeededTournament.Generate(ruleset: Ruleset.Osu);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: Ruleset.Osu);
        GameScore score = SeededScore.Generate(score: 50000, mods: mods, ruleset: Ruleset.Osu, game: game);

        // Act
        ScoreRejectionReason result = _checker.Process(score, tournament.Ruleset);

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
        Tournament tournament = SeededTournament.Generate(ruleset: tournamentRuleset);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: scoreRuleset);
        GameScore score = SeededScore.Generate(score: 50000, ruleset: scoreRuleset, mods: Mods.None, game: game);

        // Act
        ScoreRejectionReason result = _checker.Process(score, tournament.Ruleset);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(500, Mods.SuddenDeath, Ruleset.Osu, Ruleset.Osu, ScoreRejectionReason.ScoreBelowMinimum | ScoreRejectionReason.InvalidMods)]
    [InlineData(500, Mods.None, Ruleset.Osu, Ruleset.Taiko, ScoreRejectionReason.ScoreBelowMinimum | ScoreRejectionReason.RulesetMismatch)]
    [InlineData(2000, Mods.Perfect, Ruleset.Taiko, Ruleset.Osu, ScoreRejectionReason.InvalidMods | ScoreRejectionReason.RulesetMismatch)]
    [InlineData(0, Mods.Relax | Mods.Autoplay, Ruleset.Catch, Ruleset.Mania4k, ScoreRejectionReason.ScoreBelowMinimum | ScoreRejectionReason.InvalidMods | ScoreRejectionReason.RulesetMismatch)]
    public void Process_ReturnsCombinedRejectionReasons_WhenMultipleFailuresOccur(int scoreAmount, Mods mods, Ruleset scoreRuleset, Ruleset tournamentRuleset, ScoreRejectionReason expected)
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(ruleset: tournamentRuleset);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: scoreRuleset);
        GameScore score = SeededScore.Generate(score: scoreAmount, ruleset: scoreRuleset, mods: mods, game: game);

        // Act
        ScoreRejectionReason result = _checker.Process(score, tournament.Ruleset);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Ruleset.Osu)]
    [InlineData(Ruleset.Taiko)]
    [InlineData(Ruleset.Catch)]
    [InlineData(Ruleset.Mania4k)]
    [InlineData(Ruleset.Mania7k)]
    public void Process_ReturnsNone_WhenAllChecksPass(Ruleset ruleset)
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(ruleset: ruleset);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: ruleset);
        GameScore score = SeededScore.Generate(score: 100000, ruleset: ruleset, mods: Mods.Hidden | Mods.HardRock, game: game);

        // Act
        ScoreRejectionReason result = _checker.Process(score, tournament.Ruleset);

        // Assert
        Assert.Equal(ScoreRejectionReason.None, result);
    }

    [Theory]
    [InlineData(1001)]
    [InlineData(int.MaxValue)]
    [InlineData(1000000)]
    public void Process_AcceptsScoresAboveMinimum(int scoreAmount)
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(ruleset: Ruleset.Osu);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: Ruleset.Osu);
        GameScore score = SeededScore.Generate(score: scoreAmount, ruleset: Ruleset.Osu, mods: Mods.None, game: game);

        // Act
        ScoreRejectionReason result = _checker.Process(score, tournament.Ruleset);

        // Assert
        Assert.Equal(ScoreRejectionReason.None, result);
    }

    [Theory]
    [InlineData(Mods.Hidden | Mods.HardRock)]
    [InlineData(Mods.DoubleTime | Mods.Hidden)]
    [InlineData(Mods.HalfTime | Mods.Easy)]
    [InlineData(Mods.Flashlight | Mods.NoFail)]
    public void Process_AcceptsValidModCombinations(Mods mods)
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(ruleset: Ruleset.Osu);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: Ruleset.Osu);
        GameScore score = SeededScore.Generate(score: 50000, ruleset: Ruleset.Osu, mods: mods, game: game);

        // Act
        ScoreRejectionReason result = _checker.Process(score, tournament.Ruleset);

        // Assert
        Assert.Equal(ScoreRejectionReason.None, result);
    }

    [Theory]
    [InlineData(Mods.SuddenDeath | Mods.Hidden)]
    [InlineData(Mods.Perfect | Mods.HardRock)]
    [InlineData(Mods.Relax | Mods.DoubleTime)]
    [InlineData(Mods.Autoplay | Mods.Hidden | Mods.HardRock)]
    [InlineData(Mods.Relax2 | Mods.NoFail)]
    public void Process_RejectsInvalidModCombinations(Mods mods)
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(ruleset: Ruleset.Osu);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: Ruleset.Osu);
        GameScore score = SeededScore.Generate(score: 50000, ruleset: Ruleset.Osu, mods: mods, game: game);

        // Act
        ScoreRejectionReason result = _checker.Process(score, tournament.Ruleset);

        // Assert
        Assert.True(result.HasFlag(ScoreRejectionReason.InvalidMods));
    }

    [Fact]
    public void Process_LogsTraceMessages_WhenProcessingScore()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(ruleset: Ruleset.Osu);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: Ruleset.Osu);
        GameScore score = SeededScore.Generate(score: 50000, ruleset: Ruleset.Osu, mods: Mods.None, game: game);

        // Act
        _checker.Process(score, tournament.Ruleset);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Trace,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing score")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Trace,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("processed with rejection reason")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Process_LogsScoreBelowMinimum_WhenScoreFails()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(ruleset: Ruleset.Osu);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: Ruleset.Osu);
        GameScore score = SeededScore.Generate(score: 500, ruleset: Ruleset.Osu, mods: Mods.None, game: game);

        // Act
        _checker.Process(score, tournament.Ruleset);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Trace,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("failed minimum score check")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Process_LogsInvalidMods_WhenModCheckFails()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(ruleset: Ruleset.Osu);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: Ruleset.Osu);
        GameScore score = SeededScore.Generate(score: 50000, ruleset: Ruleset.Osu, mods: Mods.SuddenDeath, game: game);

        // Act
        _checker.Process(score, tournament.Ruleset);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Trace,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("failed mod check")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Process_LogsRulesetMismatch_WhenRulesetCheckFails()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(ruleset: Ruleset.Osu);
        Match match = SeededMatch.Generate(tournament: tournament);
        Game game = SeededGame.Generate(match: match, ruleset: Ruleset.Taiko);
        GameScore score = SeededScore.Generate(score: 50000, ruleset: Ruleset.Taiko, mods: Mods.None, game: game);

        // Act
        _checker.Process(score, tournament.Ruleset);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Trace,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("failed ruleset check")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
