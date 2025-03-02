using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks.Games;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Games;

public class GameScoringTypeCheckTests : AutomationChecksTestBase<GameScoringTypeCheck>
{
    [Theory]
    [InlineData(ScoringType.Score, false)]
    [InlineData(ScoringType.Accuracy, false)]
    [InlineData(ScoringType.Combo, false)]
    [InlineData(ScoringType.ScoreV2, true)]
    public void Check_PassesWhenExpected(ScoringType scoringType, bool expectedPass)
    {
        // Arrange
        Game game = SeededGame.Generate(scoringType: scoringType, rejectionReason: GameRejectionReason.None);

        GameRejectionReason expectedRejectionReason = expectedPass
            ? GameRejectionReason.None
            : GameRejectionReason.InvalidScoringType;

        // Act
        var actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedRejectionReason, game.RejectionReason);
    }
}
