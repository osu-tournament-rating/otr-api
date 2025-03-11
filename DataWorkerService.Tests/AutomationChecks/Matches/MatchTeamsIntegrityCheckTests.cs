using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks.Matches;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Matches;

public class MatchTeamsIntegrityCheckTests : AutomationChecksTestBase<MatchTeamsIntegrityCheck>
{
    [Fact]
    public void Check_WithSamePlayerInBothTeams_PassesWithWarning_SamePlayerInBothTeams()
    {
        // Arrange
        Match match = SeededMatch.Generate(
            rejectionReason: MatchRejectionReason.None,
            warningFlags: MatchWarningFlags.None);

        Game firstGame = SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);
        Game secondGame = SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);

        Player[] teamRedPlayers =
        [
            SeededPlayer.Generate(),
            SeededPlayer.Generate()
        ];
        Player[] teamBluePlayers =
        [
            SeededPlayer.Generate(),
            SeededPlayer.Generate()
        ];

        // first game is correct
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[0], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[1], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[0], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[1], game: firstGame);

        // second game first players are swapped
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamRedPlayers[0], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[1], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamBluePlayers[0], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[1], game: secondGame);

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchWarningFlags.SamePlayerInBothTeams, match.WarningFlags);
    }

    [Fact]
    public void Check_WithDifferentPlayersInBothTeams_PassesWithoutWarning()
    {
        // Arrange
        Match match = SeededMatch.Generate(
            rejectionReason: MatchRejectionReason.None,
            warningFlags: MatchWarningFlags.None);

        Game firstGame = SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);
        Game secondGame = SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);

        Player[] teamRedPlayers =
        [
            SeededPlayer.Generate(),
            SeededPlayer.Generate()
        ];
        Player[] teamBluePlayers =
        [
            SeededPlayer.Generate(),
            SeededPlayer.Generate()
        ];

        // first game is correct
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[0], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[1], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[0], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[1], game: firstGame);

        // second game is correct
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[0], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[1], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[0], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[1], game: secondGame);

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchWarningFlags.None, match.WarningFlags);
    }

    [Fact]
    public void Check_WithInvalidGameAndSamePlayerInBothTeams_PassesWithoutWarning()
    {
        // Arrange
        Match match = SeededMatch.Generate(
            rejectionReason: MatchRejectionReason.None,
            warningFlags: MatchWarningFlags.None);

        Game firstGame = SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);
        Game secondGame = SeededGame.Generate(verificationStatus: VerificationStatus.Rejected, match: match);

        Player[] teamRedPlayers =
        [
            SeededPlayer.Generate(),
            SeededPlayer.Generate()
        ];
        Player[] teamBluePlayers =
        [
            SeededPlayer.Generate(),
            SeededPlayer.Generate()
        ];

        // first game is correct
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[0], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[1], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[0], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[1], game: firstGame);

        // second game first players are swapped
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamRedPlayers[0], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[1], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamBluePlayers[0], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[1], game: secondGame);

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchWarningFlags.None, match.WarningFlags);
    }

    [Fact]
    public void Check_WithInvalidScoresAndSamePlayerInBothTeams_PassesWithoutWarning()
    {
        // Arrange
        Match match = SeededMatch.Generate(
            rejectionReason: MatchRejectionReason.None,
            warningFlags: MatchWarningFlags.None);

        Game firstGame = SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);
        Game secondGame = SeededGame.Generate(verificationStatus: VerificationStatus.Verified, match: match);

        Player[] teamRedPlayers =
        [
            SeededPlayer.Generate(),
            SeededPlayer.Generate()
        ];
        Player[] teamBluePlayers =
        [
            SeededPlayer.Generate(),
            SeededPlayer.Generate()
        ];

        // first game is correct
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[0], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[1], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[0], game: firstGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[1], game: firstGame);

        // second game first players are swapped
        SeededScore.Generate(verificationStatus: VerificationStatus.Rejected, team: Team.Blue, player: teamRedPlayers[0], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Red, player: teamRedPlayers[1], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Rejected, team: Team.Red, player: teamBluePlayers[0], game: secondGame);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, team: Team.Blue, player: teamBluePlayers[1], game: secondGame);

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchWarningFlags.None, match.WarningFlags);
    }
}
