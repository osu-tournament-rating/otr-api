using Database.Entities;
using Database.Enums.Verification;
using DataWorkerService.AutomationChecks.Matches;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Matches;

public class MatchGameCountCheckTests : AutomationChecksTestBase<MatchGameCountCheck>
{
    [Fact]
    public void Check_GivenNoGames_FailsWith_NoGames()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.False(actualPass);
        Assert.Empty(match.Games);
        Assert.Equal(MatchRejectionReason.NoGames, match.RejectionReason);
    }

    [Fact]
    public void Check_GivenOnlyPreRejectedAndRejectedGames_FailsWith_NoValidGames()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);

        SeededGame.Generate(verificationStatus: VerificationStatus.Rejected, match: match);
        SeededGame.Generate(verificationStatus: VerificationStatus.Rejected, match: match);
        SeededGame.Generate(verificationStatus: VerificationStatus.PreRejected, match: match);
        SeededGame.Generate(verificationStatus: VerificationStatus.PreRejected, match: match);

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.False(actualPass);
        Assert.DoesNotContain(match.Games, game =>
            game.VerificationStatus is VerificationStatus.PreVerified or VerificationStatus.Verified
        );
        Assert.Equal(MatchRejectionReason.NoValidGames, match.RejectionReason);
    }

    [Fact]
    public void Check_GivenPreVerifiedAndVerifiedGames_ConsidersValidGames()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);

        SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);
        SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);
        SeededGame.Generate(verificationStatus: VerificationStatus.PreVerified, match: match);
        SeededGame.Generate(verificationStatus: VerificationStatus.PreVerified, match: match);

        // Act
        AutomationCheck.Check(match);

        // Assert
        Assert.NotEqual(MatchRejectionReason.NoGames, match.RejectionReason);
        Assert.NotEqual(MatchRejectionReason.NoValidGames, match.RejectionReason);
    }

    [Theory]
    [InlineData(0, 0, false, MatchRejectionReason.NoGames)]
    [InlineData(0, 2, false, MatchRejectionReason.NoValidGames)]
    [InlineData(1, 1, false, MatchRejectionReason.UnexpectedGameCount)]
    [InlineData(2, 2, false, MatchRejectionReason.UnexpectedGameCount)]
    [InlineData(3, 3, true, MatchRejectionReason.None)]
    [InlineData(3, 7, true, MatchRejectionReason.None)]
    [InlineData(4, 3, true, MatchRejectionReason.None)]
    [InlineData(10, 0, true, MatchRejectionReason.None)]
    [InlineData(7, 7, true, MatchRejectionReason.None)]
    [InlineData(9, 1, true, MatchRejectionReason.None)]
    public void Check_PassesWhenExpected(
        int verifiedGames,
        int rejectedGames,
        bool expectedPass,
        MatchRejectionReason expectedRejectionReason
    )
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);

        if (verifiedGames >= 1)
        {
            foreach (var _ in Enumerable.Range(1, verifiedGames))
            {
                SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);
            }
        }

        if (rejectedGames >= 1)
        {
            foreach (var _ in Enumerable.Range(1, rejectedGames))
            {
                SeededGame.Generate(verificationStatus: VerificationStatus.Rejected, match: match);
            }
        }

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedRejectionReason, match.RejectionReason);
    }

    [Fact]
    public void Check_Passes_WhenApplyingWarningFlag()
    {
        // Arrange
        Match match = SeededMatch.Generate(warningFlags: MatchWarningFlags.None);

        SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);
        SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);
        SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);
        SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);

        // Act
        var passed = AutomationCheck.Check(match);

        // Assert
        Assert.Equal(MatchWarningFlags.LowGameCount, match.WarningFlags);
        Assert.True(passed);
    }

    [Theory]
    [InlineData(3, false)]
    [InlineData(4, true)]
    [InlineData(5, true)]
    [InlineData(6, false)]
    public void Check_AppliesWarningFlag_WhenExpected(int gameCount, bool expectedWarning)
    {
        // Arrange
        Match match = SeededMatch.Generate(warningFlags: MatchWarningFlags.None);

        foreach (var _ in Enumerable.Range(1, gameCount))
        {
            SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);
        }

        // Act
        AutomationCheck.Check(match);

        // Assert
        Assert.Equal(expectedWarning, match.WarningFlags == MatchWarningFlags.LowGameCount);
    }
}
