using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks.Matches;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Matches;

public class MatchEndTimeCheckTests : AutomationChecksTestBase<MatchEndTimeCheck>
{
    [Theory]
    [ClassData(typeof(SharedTestData.EndTimeNullableTestData))]
    public void Check_PassesWhenExpected(DateTime? endTime, bool expectedPass)
    {
        // Arrange
        MatchRejectionReason expectedRejectionReason = expectedPass
            ? MatchRejectionReason.None
            : MatchRejectionReason.NoEndTime;

        Match match = SeededMatch.Generate(endTime: endTime, rejectionReason: MatchRejectionReason.None);

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedRejectionReason, match.RejectionReason);
    }

    [Fact]
    public void Check_GivenEmptyDateTime_FailsWith_NoEndTime()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.EndTime = null;

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.False(actualPass);
        Assert.Equal(MatchRejectionReason.NoEndTime, match.RejectionReason);
    }
}
