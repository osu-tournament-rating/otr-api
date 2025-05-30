using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.Processors.Games;
using DataWorkerService.Utilities;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.Stats;

public class GameStatsProcessorTests
{
    private static GameStatsProcessor CreateProcessor() =>
        new(new SerilogLoggerFactory().CreateLogger<GameStatsProcessor>());

    private static GameScore[] CreateTestScores(Game game, bool verified = true, ScoreProcessingStatus processingStatus = ScoreProcessingStatus.Done)
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
            score.ProcessingStatus = processingStatus;
        }

        return scores;
    }

    [Fact]
    public void AssignScorePlacements_WithValidScores_AssignsCorrectPlacements()
    {
        // Arrange
        GameScore[] scores = CreateTestScores(SeededGame.Generate(id: 1));

        // Act
        GameStatsProcessor.AssignScorePlacements(scores);

        // Assert
        Assert.All(scores, s => Assert.Equal(s.Placement, s.Id));
    }

    [Fact]
    public void AssignScorePlacements_WithTiedScores_AssignsSequentialPlacements()
    {
        // Arrange
        GameScore[] scores =
        [
            SeededScore.Generate(id: 1, score: 500),
            SeededScore.Generate(id: 2, score: 500),
            SeededScore.Generate(id: 3, score: 400)
        ];

        // Act
        GameStatsProcessor.AssignScorePlacements(scores);

        // Assert
        Assert.Equal(1, scores[0].Placement);
        Assert.Equal(2, scores[1].Placement);
        Assert.Equal(3, scores[2].Placement);
    }

    [Fact]
    public void AssignScorePlacements_WithEmptyCollection_DoesNotThrow()
    {
        // Arrange
        GameScore[] scores = [];

        // Act & Assert
        Exception? exception = Record.Exception(() => GameStatsProcessor.AssignScorePlacements(scores));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ProcessAsync_WithValidGame_ProcessesSuccessfully()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.ProcessingStatus = GameProcessingStatus.NeedsStatCalculation;
        game.Scores = CreateTestScores(game);

        GameStatsProcessor processor = CreateProcessor();

        // Act
        await processor.ProcessAsync(game, default);

        // Assert
        Assert.Equal(GameProcessingStatus.Done, game.ProcessingStatus);
        Assert.NotEmpty(game.Rosters);
        Assert.Equal(2, game.Rosters.Count);
        Assert.All(game.Scores, s => Assert.True(s.Placement > 0));
    }

    [Fact]
    public async Task ProcessAsync_WithWrongProcessingStatus_SkipsProcessing()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.ProcessingStatus = GameProcessingStatus.Done;
        game.Scores = CreateTestScores(game);

        GameStatsProcessor processor = CreateProcessor();

        // Act
        await processor.ProcessAsync(game, default);

        // Assert
        Assert.Equal(GameProcessingStatus.Done, game.ProcessingStatus);
        Assert.Empty(game.Rosters);
    }

    [Fact]
    public async Task ProcessAsync_WithUnverifiedScores_IgnoresUnverifiedScores()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.ProcessingStatus = GameProcessingStatus.NeedsStatCalculation;

        GameScore[] scores = CreateTestScores(game, verified: false);
        scores[0].VerificationStatus = VerificationStatus.Verified; // Only one verified score
        game.Scores = scores;

        GameStatsProcessor processor = CreateProcessor();

        // Act
        await processor.ProcessAsync(game, default);

        // Assert
        Assert.Equal(GameProcessingStatus.Done, game.ProcessingStatus);
        Assert.Single(game.Rosters); // Only one team should have a roster
    }

    [Fact]
    public async Task ProcessAsync_WithIncompleteScoreProcessing_IgnoresIncompleteScores()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.ProcessingStatus = GameProcessingStatus.NeedsStatCalculation;
        game.Scores = CreateTestScores(game, processingStatus: ScoreProcessingStatus.NeedsAutomationChecks);

        GameStatsProcessor processor = CreateProcessor();

        // Act
        await processor.ProcessAsync(game, default);

        // Assert
        Assert.Equal(GameProcessingStatus.Done, game.ProcessingStatus);
        Assert.Empty(game.Rosters);
    }

    [Fact]
    public async Task ProcessAsync_WithNoValidScores_CompletesWithoutRosters()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        game.ProcessingStatus = GameProcessingStatus.NeedsStatCalculation;
        game.Scores = [];

        GameStatsProcessor processor = CreateProcessor();

        // Act
        await processor.ProcessAsync(game, default);

        // Assert
        Assert.Equal(GameProcessingStatus.Done, game.ProcessingStatus);
        Assert.Empty(game.Rosters);
    }
}
