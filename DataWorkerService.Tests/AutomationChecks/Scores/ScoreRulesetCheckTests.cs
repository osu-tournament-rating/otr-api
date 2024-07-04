using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using DataWorkerService.AutomationChecks.Scores;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Scores;

public class ScoreRulesetCheckTests : AutomationChecksTestBase<ScoreRulesetCheck>
{
    [Theory]
    [ClassData(typeof(SharedTestData.RulesetTestData))]
    public void Check_PassesWhenExpected(Ruleset scoreRuleset, Ruleset tournamentRuleset, bool expectedPass)
    {
        // Arrange
        GameScore score = SeededScore.Generate(ruleset: scoreRuleset, rejectionReason: ScoreRejectionReason.None);
        score.Game.Match.Tournament.Ruleset = tournamentRuleset;

        ScoreRejectionReason expectedRejectionReason = expectedPass
            ? ScoreRejectionReason.None
            : ScoreRejectionReason.RulesetMismatch;

        // Act
        var actualPass = AutomationCheck.Check(score);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedRejectionReason, score.RejectionReason);
    }
}
