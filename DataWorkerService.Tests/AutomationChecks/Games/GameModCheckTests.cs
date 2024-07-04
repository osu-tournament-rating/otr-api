using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using DataWorkerService.AutomationChecks.Games;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Games;

public class GameModCheckTests : AutomationChecksTestBase<GameModCheck>
{
    [Theory]
    [ClassData(typeof(SharedTestData.ModTestData))]
    public void Check_PassesWhenExpected(Mods mods, bool expectedPass)
    {
        // Arrange
        Game game = SeededGame.Generate(mods: mods, rejectionReason: GameRejectionReason.None);

        GameRejectionReason expectedRejectionReason = expectedPass
            ? GameRejectionReason.None
            : GameRejectionReason.InvalidMods;

        // Act
        var actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedRejectionReason, game.RejectionReason);
    }
}
