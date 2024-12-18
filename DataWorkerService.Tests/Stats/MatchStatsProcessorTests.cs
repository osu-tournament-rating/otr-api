using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using DataWorkerService.Processors;
using DataWorkerService.Processors.Games;
using DataWorkerService.Processors.Matches;
using DataWorkerService.Tests.Mocks;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.Stats;

public class MatchStatsProcessorTests
{
    [Fact]
    public void Processor_ProperlyCreates_MatchWinRecord()
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

            GameStatsProcessor.AssignScorePlacements(game.Scores);
            game.WinRecord = GameStatsProcessor.GenerateWinRecord(game.Scores);
        }

        const Team expectedWinningTeam = Team.Blue;
        const Team expectedLosingTeam = Team.Red;

        // Act
        MatchWinRecord result = MatchStatsProcessor.GenerateWinRecord(match.Games);

        // Assert
        Assert.Equal(expectedWinningTeam, result.WinnerTeam);
        Assert.Equal(expectedLosingTeam, result.LoserTeam);
        Assert.Distinct(result.WinnerRoster);
        Assert.Distinct(result.LoserRoster);
    }

    [Fact]
    public async Task Processor_MovesGamesToNextProcessingStatus()
    {
        Match match = SeededMatch.ExampleMatch();

        match.ProcessingStatus = MatchProcessingStatus.NeedsStatCalculation;
        match.VerificationStatus = VerificationStatus.Verified;
        foreach (Game game in match.Games)
        {
            game.VerificationStatus = VerificationStatus.Verified;
            game.ProcessingStatus = GameProcessingStatus.NeedsStatCalculation;
            foreach (GameScore score in game.Scores)
            {
                score.VerificationStatus = VerificationStatus.Verified;
                score.ProcessingStatus = ScoreProcessingStatus.Done;
            }
        }

        // Act
        IProcessor<Match> processor = MockResolvers.MatchProcessorResolver.GetStatsProcessor();
        await processor.ProcessAsync(match, default);

        // Assert
        Assert.Equal(MatchProcessingStatus.NeedsRatingProcessorData, match.ProcessingStatus);
        Assert.All(match.Games, g => { Assert.Equal(GameProcessingStatus.Done, g.ProcessingStatus); });
    }
}
