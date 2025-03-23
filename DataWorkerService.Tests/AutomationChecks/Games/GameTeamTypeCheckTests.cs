using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks.Games;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Games;

public class GameTeamTypeCheckTests : AutomationChecksTestBase<GameTeamTypeCheck>
{
    [Theory]
    [InlineData(TeamType.HeadToHead, false)]
    [InlineData(TeamType.TagCoop, false)]
    [InlineData(TeamType.TeamVs, true)]
    [InlineData(TeamType.TagTeamVs, false)]
    public void Check_PassesWhenExpected(TeamType teamType, bool expectedPass)
    {
        // Arrange
        Game game = SeededGame.Generate(teamType: teamType, rejectionReason: GameRejectionReason.None);

        GameRejectionReason expectedRejectionReason = expectedPass
            ? GameRejectionReason.None
            : GameRejectionReason.InvalidTeamType;

        // Act
        var actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedRejectionReason, game.RejectionReason);
    }
}
