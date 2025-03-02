using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;
using DataWorkerService.Processors.Games;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.Stats;

public class GameStatsProcessorTests
{
    [Fact]
    public void Processor_ProperlyAssigns_ScorePlacements()
    {
        // Arrange
        GameScore[] scores =
        [
            SeededScore.Generate(id: 1, score: 600),
            SeededScore.Generate(id: 2, score: 500),
            SeededScore.Generate(id: 3, score: 400),
            SeededScore.Generate(id: 4, score: 300),
            SeededScore.Generate(id: 5, score: 200),
            SeededScore.Generate(id: 6, score: 100)
        ];

        // Act
        GameStatsProcessor.AssignScorePlacements(scores);

        // Assert
        Assert.All(scores, s => Assert.Equal(s.Placement, s.Id));
    }

    [Fact]
    public void Processor_ProperlyCreates_Rosters()
    {
        Game game = SeededGame.Generate(id: 1);

        // Arrange
        GameScore[] scores =
        [
            SeededScore.Generate(id: 1, score: 600, team: Team.Red, player: SeededPlayer.Generate(id: 1), game: game),
            SeededScore.Generate(id: 2, score: 500, team: Team.Red, player: SeededPlayer.Generate(id: 2), game: game),
            SeededScore.Generate(id: 3, score: 400, team: Team.Red, player: SeededPlayer.Generate(id: 3), game: game),
            SeededScore.Generate(id: 4, score: 300, team: Team.Blue, player: SeededPlayer.Generate(id: 4), game: game),
            SeededScore.Generate(id: 5, score: 200, team: Team.Blue, player: SeededPlayer.Generate(id: 5), game: game),
            SeededScore.Generate(id: 6, score: 100, team: Team.Blue, player: SeededPlayer.Generate(id: 6), game: game)
        ];

        // Act
        ICollection<GameRoster> rosters = GameStatsProcessor.GenerateRosters(scores);

        GameRoster redRoster = rosters.First(r => r.Team == Team.Red);
        GameRoster blueRoster = rosters.First(r => r.Team == Team.Blue);

        // Assert
        Assert.Equal(Team.Red, redRoster.Team);
        Assert.Equal([1, 2, 3], redRoster.Roster);
        Assert.Equal(600 + 500 + 400, redRoster.Score);

        Assert.Equal(Team.Blue, blueRoster.Team);
        Assert.Equal([4, 5, 6], blueRoster.Roster);
        Assert.Equal(300 + 200 + 100, blueRoster.Score);

        Assert.Equal(2, rosters.Count);
    }

    [Fact]
    public async Task Processor_Integrated_BehavesAsExpected()
    {
        // Arrange
        var processor = new GameStatsProcessor(new Logger<GameStatsProcessor>(new SerilogLoggerFactory()));

        Game game = SeededGame.Generate(
            verificationStatus: VerificationStatus.Verified,
            processingStatus: GameProcessingStatus.NeedsStatCalculation
        );

        SeededScore.Generate(
            id: 1,
            score: 600,
            team: Team.Blue,
            verificationStatus: VerificationStatus.Verified,
            processingStatus: ScoreProcessingStatus.Done,
            player: SeededPlayer.Generate(id: 1),
            game: game
        );
        SeededScore.Generate(
            id: 2,
            score: 300,
            team: Team.Red,
            verificationStatus: VerificationStatus.Verified,
            processingStatus: ScoreProcessingStatus.Done,
            player: SeededPlayer.Generate(id: 2),
            game: game
        );

        // Act
        await processor.ProcessAsync(game, new CancellationToken());

        // Assert
        Assert.Equal(GameProcessingStatus.Done, game.ProcessingStatus);
        Assert.NotNull(game.Rosters);
    }
}
