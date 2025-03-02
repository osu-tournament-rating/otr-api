using Common.Enums.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks;
using DataWorkerService.AutomationChecks.Scores;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Scores;

public class ScoreMinimumCheckTests : AutomationChecksTestBase<ScoreMinimumCheck>
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(Constants.ScoreMinimum - 1, false)]
    [InlineData(Constants.ScoreMinimum, false)]
    [InlineData(Constants.ScoreMinimum + 1, true)]
    [InlineData(124_961, true)]
    [InlineData(873_884, true)]
    public void Check_PassesWhenExpected(int score, bool expectedPass)
    {
        // Arrange
        GameScore gameScore = SeededScore.Generate(score: score, rejectionReason: ScoreRejectionReason.None);

        ScoreRejectionReason expectedRejectionReason = expectedPass
            ? ScoreRejectionReason.None
            : ScoreRejectionReason.ScoreBelowMinimum;

        // Act
        var actualPass = AutomationCheck.Check(gameScore);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedRejectionReason, gameScore.RejectionReason);
    }
}
