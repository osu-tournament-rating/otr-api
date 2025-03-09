using Database.Entities;
using Database.Enums.Verification;
using DataWorkerService.AutomationChecks.Tournaments;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Tournaments;

public class TournamentMatchCountCheckTests : AutomationChecksTestBase<TournamentMatchCountCheck>
{
    [Fact]
    public void Check_GivenNoVerifiedMatches_FailsWith_NoVerifiedMatches()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(rejectionReason: TournamentRejectionReason.None);

        SeededMatch.Generate(verificationStatus: VerificationStatus.PreRejected, tournament: tournament);
        SeededMatch.Generate(verificationStatus: VerificationStatus.Rejected, tournament: tournament);

        // Act
        var actualPass = AutomationCheck.Check(tournament);

        // Assert
        Assert.False(actualPass);
        Assert.Equal(TournamentRejectionReason.NoVerifiedMatches, tournament.RejectionReason);
    }

    [Theory]
    [InlineData(1, 0, false)] // 0%
    [InlineData(2, 1, false)] // 50%
    [InlineData(3, 2, false)] // 66%
    [InlineData(10, 7, false)] // 70%
    [InlineData(20, 15, false)] // 75%
    [InlineData(10, 8, true)] // 80%
    [InlineData(20, 17, true)] // 85%
    [InlineData(10, 10, true)] // 100%
    public void Check_GivenVerifiedMatches_PassesWhenExpected(
        int totalMatchCount,
        int verifiedMatchCount,
        bool expectedPass
    )
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(rejectionReason: TournamentRejectionReason.None);

        if (verifiedMatchCount >= 1)
        {
            foreach (var _ in Enumerable.Range(1, verifiedMatchCount))
            {
                SeededMatch.Generate(verificationStatus: VerificationStatus.Verified, tournament: tournament);
            }
        }

        if (totalMatchCount - verifiedMatchCount >= 1)
        {
            foreach (var _ in Enumerable.Range(1, totalMatchCount - verifiedMatchCount))
            {
                SeededMatch.Generate(
                    verificationStatus: VerificationStatus.Rejected,
                    tournament: tournament,
                    rejectionReason: MatchRejectionReason.NoValidGames);
            }
        }

        // Act
        var actualPass = AutomationCheck.Check(tournament);

        // Assert
        Assert.Equal(expectedPass, actualPass);
    }
}
