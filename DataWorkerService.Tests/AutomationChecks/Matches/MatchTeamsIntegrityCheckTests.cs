using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks.Matches;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Matches;

public class MatchTeamsIntegrityCheckTests : AutomationChecksTestBase<MatchTeamsIntegrityCheck>
{
    [Fact]
    public void Check_WithSamePlayerInBothTeams_PassesWithWarning_OverlappingRosters()
    {
        GameAttributes[] games =
        [
            new(VerificationStatus.Verified),
            new(VerificationStatus.Verified)
        ];

        ScoreAttributes[] scores =
        [
            // first game is correct
            new(VerificationStatus.Verified, Team.Red, Team.Red, 0, 0),
            new(VerificationStatus.Verified, Team.Red, Team.Red, 1, 0),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 0, 0),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 1, 0),
            // second game first players are swapped
            new(VerificationStatus.Verified, Team.Blue, Team.Red, 0, 1),
            new(VerificationStatus.Verified, Team.Red, Team.Red, 1, 1),
            new(VerificationStatus.Verified, Team.Red, Team.Blue, 0, 1),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 1, 1)
        ];

        AssertForMatch(games, scores, true, MatchWarningFlags.OverlappingRosters);
    }

    [Fact]
    public void Check_WithDifferentPlayersInBothTeams_PassesWithoutWarning()
    {
        GameAttributes[] games =
        [
            new(VerificationStatus.Verified),
            new(VerificationStatus.Verified)
        ];

        ScoreAttributes[] scores =
        [
            // first game is correct
            new(VerificationStatus.Verified, Team.Red, Team.Red, 0, 0),
            new(VerificationStatus.Verified, Team.Red, Team.Red, 1, 0),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 0, 0),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 1, 0),
            // second game is correct
            new(VerificationStatus.Verified, Team.Red, Team.Red, 0, 1),
            new(VerificationStatus.Verified, Team.Red, Team.Red, 1, 1),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 0, 1),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 1, 1)
        ];

        AssertForMatch(games, scores, true, MatchWarningFlags.None);
    }

    [Fact]
    public void Check_WithInvalidGameAndSamePlayerInBothTeams_PassesWithoutWarning()
    {
        GameAttributes[] games =
        [
            new(VerificationStatus.Verified),
            new(VerificationStatus.Rejected)
        ];

        ScoreAttributes[] scores =
        [
            // first game is correct
            new(VerificationStatus.Verified, Team.Red, Team.Red, 0, 0),
            new(VerificationStatus.Verified, Team.Red, Team.Red, 1, 0),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 0, 0),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 1, 0),
            // second game first players are swapped
            new(VerificationStatus.Verified, Team.Blue, Team.Red, 0, 1),
            new(VerificationStatus.Verified, Team.Red, Team.Red, 1, 1),
            new(VerificationStatus.Verified, Team.Red, Team.Blue, 0, 1),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 1, 1)
        ];

        AssertForMatch(games, scores, true, MatchWarningFlags.None);
    }

    [Fact]
    public void Check_WithInvalidScoresAndSamePlayerInBothTeams_PassesWithoutWarning()
    {
        GameAttributes[] games =
        [
            new(VerificationStatus.Verified),
            new(VerificationStatus.Verified)
        ];

        ScoreAttributes[] scores =
        [
            // first game is correct
            new(VerificationStatus.Verified, Team.Red, Team.Red, 0, 0),
            new(VerificationStatus.Verified, Team.Red, Team.Red, 1, 0),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 0, 0),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 1, 0),
            // second game first players are swapped
            new(VerificationStatus.Rejected, Team.Blue, Team.Red, 0, 1),
            new(VerificationStatus.Verified, Team.Red, Team.Red, 1, 1),
            new(VerificationStatus.Rejected, Team.Red, Team.Blue, 0, 1),
            new(VerificationStatus.Verified, Team.Blue, Team.Blue, 1, 1)
        ];

        AssertForMatch(games, scores, true, MatchWarningFlags.None);
    }

    private static void AssertForMatch(
        GameAttributes[] gameAttributes,
        ScoreAttributes[] scoreAttributes,
        bool expectedPass,
        MatchWarningFlags expectedWarningFlags,
        int teamLobbySize = 2)
    {
        Match match = SeededMatch.Generate(
            rejectionReason: MatchRejectionReason.None,
            warningFlags: MatchWarningFlags.None);

        Game[] games = gameAttributes
            .Select(gameAttribute =>
                SeededGame.Generate(verificationStatus: gameAttribute.VerificationStatus, match: match))
            .ToArray();

        Dictionary<Team, Player[]> players = GeneratePlayers(teamLobbySize);

        GameScore[] _ = scoreAttributes
            .Select(scoreAttribute => SeededScore.Generate(
                verificationStatus: scoreAttribute.VerificationStatus,
                team: scoreAttribute.ScoreTeam,
                player: players[scoreAttribute.PlayerTeam][scoreAttribute.PlayerIndex],
                game: games[scoreAttribute.GameIndex]))
            .ToArray();

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedWarningFlags, match.WarningFlags);
    }

    private static Dictionary<Team, Player[]> GeneratePlayers(int teamLobbySize) =>
        new()
        {
            [Team.Red] = Enumerable.Range(0, teamLobbySize)
                .Select(i => SeededPlayer.Generate())
                .ToArray(),
            [Team.Blue] = Enumerable.Range(0, teamLobbySize)
                .Select(i => SeededPlayer.Generate())
                .ToArray()
        };

    private record GameAttributes(VerificationStatus VerificationStatus);

    private record ScoreAttributes(
        VerificationStatus VerificationStatus,
        Team ScoreTeam,
        Team PlayerTeam,
        int PlayerIndex,
        int GameIndex);
}
