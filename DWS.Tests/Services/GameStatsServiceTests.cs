using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;
using TestingUtils.SeededData;

namespace DWS.Tests.Services;

/// <summary>
/// Unit tests for the GameStatsService class.
/// </summary>
public class GameStatsServiceTests
{
    private readonly Mock<ILogger<GameStatsService>> _loggerMock = new();
    private readonly GameStatsService _service;

    public GameStatsServiceTests()
    {
        _service = new GameStatsService(_loggerMock.Object);
    }

    /// <summary>
    /// Creates test scores for a game.
    /// </summary>
    private static GameScore[] CreateTestScores(Game game, bool verified = true)
    {
        GameScore[] scores =
        [
            SeededScore.Generate(id: 1, score: 600, team: Team.Red, player: SeededPlayer.Generate(id: 1), game: game),
            SeededScore.Generate(id: 2, score: 500, team: Team.Red, player: SeededPlayer.Generate(id: 2), game: game),
            SeededScore.Generate(id: 3, score: 400, team: Team.Red, player: SeededPlayer.Generate(id: 3), game: game),
            SeededScore.Generate(id: 4, score: 300, team: Team.Blue, player: SeededPlayer.Generate(id: 4), game: game),
            SeededScore.Generate(id: 5, score: 200, team: Team.Blue, player: SeededPlayer.Generate(id: 5), game: game),
            SeededScore.Generate(id: 6, score: 100, team: Team.Blue, player: SeededPlayer.Generate(id: 6), game: game)
        ];

        foreach (GameScore score in scores)
        {
            score.VerificationStatus = verified ? VerificationStatus.Verified : VerificationStatus.None;
        }

        return scores;
    }

    [Fact]
    public async Task ProcessGameStatsAsync_WithUnverifiedGame_ReturnsFalse()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.VerificationStatus = VerificationStatus.None;
        game.Scores = CreateTestScores(game);

        // Act
        bool result = await _service.ProcessGameStatsAsync(game);

        // Assert
        Assert.False(result);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("unverified game")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessGameStatsAsync_WithVerifiedGame_AssignsCorrectPlacements()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.VerificationStatus = VerificationStatus.Verified;
        game.Scores = CreateTestScores(game);

        // Act
        bool result = await _service.ProcessGameStatsAsync(game);

        // Assert
        Assert.True(result);
        Assert.Equal(1, game.Scores.First(s => s.Id == 1).Placement);
        Assert.Equal(2, game.Scores.First(s => s.Id == 2).Placement);
        Assert.Equal(3, game.Scores.First(s => s.Id == 3).Placement);
        Assert.Equal(4, game.Scores.First(s => s.Id == 4).Placement);
        Assert.Equal(5, game.Scores.First(s => s.Id == 5).Placement);
        Assert.Equal(6, game.Scores.First(s => s.Id == 6).Placement);
    }

    [Fact]
    public async Task ProcessGameStatsAsync_WithVerifiedGame_GeneratesRosters()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.VerificationStatus = VerificationStatus.Verified;
        game.Scores = CreateTestScores(game);

        // Act
        bool result = await _service.ProcessGameStatsAsync(game);

        // Assert
        Assert.True(result);
        Assert.NotEmpty(game.Rosters);
        Assert.Equal(2, game.Rosters.Count);

        GameRoster? redRoster = game.Rosters.FirstOrDefault(r => r.Team == Team.Red);
        GameRoster? blueRoster = game.Rosters.FirstOrDefault(r => r.Team == Team.Blue);

        Assert.NotNull(redRoster);
        Assert.NotNull(blueRoster);
        Assert.Equal(3, redRoster.Roster.Length);
        Assert.Equal(3, blueRoster.Roster.Length);
        Assert.Equal(1500, redRoster.Score); // 600 + 500 + 400
        Assert.Equal(600, blueRoster.Score);  // 300 + 200 + 100
    }

    [Fact]
    public async Task ProcessGameStatsAsync_WithMixedVerificationStatus_OnlyProcessesVerifiedScores()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.VerificationStatus = VerificationStatus.Verified;
        game.Scores = CreateTestScores(game);

        // Mark some scores as unverified
        game.Scores.First(s => s.Id == 2).VerificationStatus = VerificationStatus.None;
        game.Scores.First(s => s.Id == 5).VerificationStatus = VerificationStatus.Rejected;

        // Act
        bool result = await _service.ProcessGameStatsAsync(game);

        // Assert
        Assert.True(result);

        // Only verified scores should have placements assigned
        var verifiedScores = game.Scores.Where(s => s.VerificationStatus == VerificationStatus.Verified).ToList();
        Assert.Equal(4, verifiedScores.Count);

        // Check rosters only contain verified players
        Assert.Equal(2, game.Rosters.Count);
        GameRoster? redRoster = game.Rosters.FirstOrDefault(r => r.Team == Team.Red);
        GameRoster? blueRoster = game.Rosters.FirstOrDefault(r => r.Team == Team.Blue);

        Assert.NotNull(redRoster);
        Assert.NotNull(blueRoster);
        Assert.Equal(2, redRoster.Roster.Length); // Only players 1 and 3
        Assert.Equal(2, blueRoster.Roster.Length); // Only players 4 and 6
    }

    [Fact]
    public async Task ProcessGameStatsAsync_SetsLastProcessingDate()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.VerificationStatus = VerificationStatus.Verified;
        game.Scores = CreateTestScores(game);
        DateTime beforeProcessing = DateTime.UtcNow;

        // Act
        bool result = await _service.ProcessGameStatsAsync(game);

        // Assert
        Assert.True(result);
        Assert.True(game.LastProcessingDate >= beforeProcessing);
        Assert.True(game.LastProcessingDate <= DateTime.UtcNow);
    }

    [Fact]
    public async Task ProcessGameStatsAsync_WithNoScores_StillGeneratesEmptyRosters()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.VerificationStatus = VerificationStatus.Verified;
        game.Scores = [];

        // Act
        bool result = await _service.ProcessGameStatsAsync(game);

        // Assert
        Assert.True(result);
        Assert.Empty(game.Rosters);
    }

    [Fact]
    public async Task ProcessGameStatsAsync_WithTiedScores_AssignsCorrectPlacements()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.VerificationStatus = VerificationStatus.Verified;
        game.Scores =
        [
            SeededScore.Generate(id: 1, score: 500, team: Team.Red, player: SeededPlayer.Generate(id: 1), game: game),
            SeededScore.Generate(id: 2, score: 500, team: Team.Red, player: SeededPlayer.Generate(id: 2), game: game),
            SeededScore.Generate(id: 3, score: 300, team: Team.Blue, player: SeededPlayer.Generate(id: 3), game: game),
        ];

        foreach (GameScore score in game.Scores)
        {
            score.VerificationStatus = VerificationStatus.Verified;
        }

        // Act
        bool result = await _service.ProcessGameStatsAsync(game);

        // Assert
        Assert.True(result);

        // Tied scores should still get sequential placements
        var orderedScores = game.Scores.OrderByDescending(s => s.Score).ToList();
        Assert.Equal(1, orderedScores[0].Placement);
        Assert.Equal(2, orderedScores[1].Placement);
        Assert.Equal(3, orderedScores[2].Placement);
    }
}
