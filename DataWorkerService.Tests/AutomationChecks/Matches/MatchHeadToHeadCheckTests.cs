using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks.Matches;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Matches;

public class MatchHeadToHeadCheckTests : AutomationChecksTestBase<MatchHeadToHeadCheck>
{
    [Fact]
    public void Check_GivenNoGames_PassesWithNoRejectionReason()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Empty(match.Games);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void Check_GivenInvalidTournamentLobbySizes_PassesWithNoRejectionReason(int tournamentLobbySize)
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbyTeamSize = tournamentLobbySize;

        SeededGame.Generate(match: match);

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
    }

    [Theory]
    // VerificationStatus not PreRejected
    [InlineData(VerificationStatus.Verified, GameRejectionReason.InvalidTeamType, TeamType.HeadToHead, 2)]
    // RejectionReason not only InvalidTeamType
    [InlineData(VerificationStatus.PreRejected, GameRejectionReason.None, TeamType.HeadToHead, 2)]
    // TeamType not HeadToHead
    [InlineData(VerificationStatus.PreRejected, GameRejectionReason.InvalidTeamType, TeamType.TagCoop, 2)]
    // Score count not 2
    [InlineData(VerificationStatus.PreRejected, GameRejectionReason.InvalidTeamType, TeamType.HeadToHead, 3)]
    public void Check_GivenGamesIneligibleForConversion_PassesWithNoRejectionReason(
        VerificationStatus gameVerificationStatus,
        GameRejectionReason gameRejectionReason,
        TeamType gameTeamType,
        int gameScoreCount
    )
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbyTeamSize = 1;

        foreach (var idx in Enumerable.Range(0, 2))
        {
            Game game = SeededGame.Generate(
                teamType: gameTeamType,
                verificationStatus: gameVerificationStatus,
                rejectionReason: gameRejectionReason,
                match: match
            );

            if (gameScoreCount <= 0)
            {
                continue;
            }

            foreach (var _ in Enumerable.Range(1, gameScoreCount))
            {
                // Set differing player osu! ids to ensure the check fails later
                // This test is only meant to test the eligibility constraints
                SeededScore.Generate(
                    verificationStatus: VerificationStatus.PreVerified,
                    rejectionReason: ScoreRejectionReason.None,
                    team: Team.NoTeam,
                    player: SeededPlayer.Generate(osuId: idx),
                    game: game
                );
            }
        }

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);
    }

    [Fact]
    public void Check_GivenGamesEligibleForConversion_WithMoreThanTwoUniquePlayers_FailsAllWithFailedTeamVsConversion()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbyTeamSize = 1;

        // Creates 2 games, 2 scores each, with players having osu! ids of 1, 2, or 3
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            verificationStatus: VerificationStatus.PreRejected,
            rejectionReason: GameRejectionReason.InvalidTeamType,
            match: match
        );
        SeededScore.Generate(
            verificationStatus: VerificationStatus.PreVerified,
            rejectionReason: ScoreRejectionReason.None,
            team: Team.NoTeam,
            player: SeededPlayer.Generate(osuId: 1),
            game: gameOne
        );
        SeededScore.Generate(
            verificationStatus: VerificationStatus.PreVerified,
            rejectionReason: ScoreRejectionReason.None,
            team: Team.NoTeam, player:
            SeededPlayer.Generate(osuId: 2),
            game: gameOne
        );

        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            verificationStatus: VerificationStatus.PreRejected,
            rejectionReason: GameRejectionReason.InvalidTeamType,
            match: match
        );
        SeededScore.Generate(
            verificationStatus: VerificationStatus.PreVerified,
            rejectionReason: ScoreRejectionReason.None,
            team: Team.NoTeam,
            player: SeededPlayer.Generate(osuId: 3),
            game: gameTwo
        );
        SeededScore.Generate(
            verificationStatus: VerificationStatus.PreVerified,
            rejectionReason: ScoreRejectionReason.None,
            team: Team.NoTeam, player:
            SeededPlayer.Generate(osuId: 4),
            game: gameTwo
        );

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.False(actualPass);
        Assert.Equal(MatchRejectionReason.FailedTeamVsConversion, match.RejectionReason);
        Assert.True(match.Games.All(g => g.RejectionReason.HasFlag(GameRejectionReason.FailedTeamVsConversion)));
    }

    [Fact]
    public void Check_PassesWhenExpected_WithConversion()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbyTeamSize = 1;

        // Creates 2 games, 2 scores each, with players having osu! ids of 1 and 2
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            verificationStatus: VerificationStatus.PreRejected,
            rejectionReason: GameRejectionReason.InvalidTeamType,
            match: match
        );
        SeededScore.Generate(
            verificationStatus: VerificationStatus.PreVerified,
            rejectionReason: ScoreRejectionReason.None,
            team: Team.NoTeam,
            player: SeededPlayer.Generate(osuId: 1),
            game: gameOne
        );
        SeededScore.Generate(
            verificationStatus: VerificationStatus.PreVerified,
            rejectionReason: ScoreRejectionReason.None,
            team: Team.NoTeam, player:
            SeededPlayer.Generate(osuId: 2),
            game: gameOne
        );

        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            verificationStatus: VerificationStatus.PreRejected,
            rejectionReason: GameRejectionReason.InvalidTeamType,
            match: match
        );
        SeededScore.Generate(
            verificationStatus: VerificationStatus.PreVerified,
            rejectionReason: ScoreRejectionReason.None,
            team: Team.NoTeam,
            player: SeededPlayer.Generate(osuId: 1),
            game: gameTwo
        );
        SeededScore.Generate(
            verificationStatus: VerificationStatus.PreVerified,
            rejectionReason: ScoreRejectionReason.None,
            team: Team.NoTeam, player:
            SeededPlayer.Generate(osuId: 2),
            game: gameTwo
        );

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);

        Assert.True(match.Games.All(g => g.TeamType is TeamType.TeamVs));
        Assert.True(match.Games.SelectMany(g => g.Scores).All(s => s.Team is Team.Blue or Team.Red));
    }
}
