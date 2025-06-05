using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks.Games;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Games;

public class GameRulesetCheckTests : AutomationChecksTestBase<GameRulesetCheck>
{
    [Theory]
    [ClassData(typeof(SharedTestData.RulesetTestData))]
    public void Check_PassesWhenExpected(Ruleset gameRuleset, Ruleset tournamentRuleset, bool expectedPass)
    {
        // Arrange
        Game game = SeededGame.Generate(ruleset: gameRuleset, rejectionReason: GameRejectionReason.None);
        game.Match.Tournament.Ruleset = tournamentRuleset;

        GameRejectionReason expectedRejectionReason = expectedPass
            ? GameRejectionReason.None
            : GameRejectionReason.RulesetMismatch;

        // Act
        bool actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedRejectionReason, game.RejectionReason);
    }
}
