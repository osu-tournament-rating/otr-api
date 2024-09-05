using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
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
    public void Processor_ProperlyCreates_GameWinRecord()
    {
        // Arrange
        const Team expectedWinningTeam = Team.Blue;
        const Team expectedLosingTeam = Team.Red;

        GameScore[] scores =
        [
            SeededScore.Generate(id: 1, score: 600, team: expectedWinningTeam, player: SeededPlayer.Generate(id: 1)),
            SeededScore.Generate(id: 2, score: 500, team: expectedWinningTeam, player: SeededPlayer.Generate(id: 2)),
            SeededScore.Generate(id: 3, score: 400, team: expectedWinningTeam, player: SeededPlayer.Generate(id: 3)),
            SeededScore.Generate(id: 4, score: 300, team: expectedLosingTeam, player: SeededPlayer.Generate(id: 4)),
            SeededScore.Generate(id: 5, score: 200, team: expectedLosingTeam, player: SeededPlayer.Generate(id: 5)),
            SeededScore.Generate(id: 6, score: 100, team: expectedLosingTeam, player: SeededPlayer.Generate(id: 6))
        ];

        // Act
        GameWinRecord result = GameStatsProcessor.GenerateWinRecord(scores);

        // Assert
        Assert.Equal(expectedWinningTeam, result.WinnerTeam);
        Assert.Equal(expectedLosingTeam, result.LoserTeam);
        Assert.Equal([1, 2, 3], result.WinnerRoster);
        Assert.Equal([4, 5, 6], result.LoserRoster);
        Assert.Equal(1500, result.WinnerScore);
        Assert.Equal(600, result.LoserScore);
    }

    [Fact]
    public async void Processor_Integrated_BehavesAsExpected()
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
        Assert.NotNull(game.WinRecord);
    }
}
