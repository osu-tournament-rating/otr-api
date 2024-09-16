using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using DataWorkerService.Processors.Games;
using DataWorkerService.Processors.Matches;
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
}
