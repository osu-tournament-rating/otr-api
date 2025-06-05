using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks.Games;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Games;

public class GameEndTimeCheckTests : AutomationChecksTestBase<GameEndTimeCheck>
{
    [Theory]
    [ClassData(typeof(SharedTestData.EndTimeTestData))]
    public void Check_PassesWhenExpected(DateTime endTime, bool expectedPass)
    {
        // Arrange
        GameRejectionReason expectedRejectionReason = expectedPass
            ? GameRejectionReason.None
            : GameRejectionReason.NoEndTime;

        Game game = SeededGame.Generate(endTime: endTime, rejectionReason: GameRejectionReason.None);

        // Act
        bool actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedRejectionReason, game.RejectionReason);
    }

    [Fact]
    public void Check_GivenDefaultDateTime_Fails()
    {
        // Arrange
        Game game = SeededGame.Generate(rejectionReason: GameRejectionReason.None);
        game.EndTime = default;

        // Act
        bool actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.False(actualPass);
        Assert.Equal(GameRejectionReason.NoEndTime, game.RejectionReason);
    }
}
