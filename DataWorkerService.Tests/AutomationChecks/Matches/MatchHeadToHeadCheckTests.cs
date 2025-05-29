using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks.Matches;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Matches;

public class MatchHeadToHeadCheckTests : AutomationChecksTestBase<MatchHeadToHeadCheck>
{
    #region Player Count Validation Tests

    [Fact]
    public void Check_GivenMatchWithMoreThanTwoPlayers_FailsAndSetsRejectionReasons()
    {
        // Arrange
        var tournament = SeededTournament.Generate(teamSize: 1);
        var match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        var game = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );

        // Add 2 scores (valid count) but with 3 different players across the match
        var player1 = SeededPlayer.Generate(id: 1);
        var player2 = SeededPlayer.Generate(id: 2);
        var player3 = SeededPlayer.Generate(id: 3);

        SeededScore.Generate(game: game, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game, player: player2, verificationStatus: VerificationStatus.Verified);

        // Add another game with the third player to make total unique players = 3
        var game2 = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: game2, player: player3, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.False(result);
        Assert.True(match.RejectionReason.HasFlag(MatchRejectionReason.FailedTeamVsConversion));
        Assert.True(game.RejectionReason.HasFlag(GameRejectionReason.FailedTeamVsConversion));
    }

    [Fact]
    public void Check_GivenMatchWithOnlyOnePlayer_FailsAndSetsRejectionReasons()
    {
        // Arrange
        var tournament = SeededTournament.Generate(teamSize: 1);
        var match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        var game = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );

        var player = SeededPlayer.Generate(id: 1);
        SeededScore.Generate(game: game, player: player, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.False(result);
        Assert.True(match.RejectionReason.HasFlag(MatchRejectionReason.FailedTeamVsConversion));
        Assert.True(game.RejectionReason.HasFlag(GameRejectionReason.FailedTeamVsConversion));
    }

    #endregion

    #region Game Player Validation Tests

    [Fact]
    public void Check_GivenGameWithUnexpectedPlayers_FailsAndSetsRejectionReasons()
    {
        // Arrange
        var tournament = SeededTournament.Generate(teamSize: 1);
        var match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        var player1 = SeededPlayer.Generate(id: 1);
        var player2 = SeededPlayer.Generate(id: 2);
        var player3 = SeededPlayer.Generate(id: 3); // Unexpected player

        // First game with expected players
        var game1 = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: game1, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game1, player: player2, verificationStatus: VerificationStatus.Verified);

        // Second game with unexpected player (this will cause validation to fail)
        var game2 = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: game2, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game2, player: player2, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game2, player: player3, verificationStatus: VerificationStatus.Verified); // This makes it 3 players total

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.False(result);
        // The match should fail due to having 3 unique players, not game validation
        Assert.True(match.RejectionReason.HasFlag(MatchRejectionReason.FailedTeamVsConversion));
        Assert.True(game1.RejectionReason.HasFlag(GameRejectionReason.FailedTeamVsConversion));
    }

    #endregion

    #region Team Assignment Tests

    [Fact]
    public void Check_GivenValidMatch_DeterminesTeamAssignmentsFromHalfwayGame()
    {
        // Arrange
        var tournament = SeededTournament.Generate(teamSize: 1);
        var match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        var player1 = SeededPlayer.Generate(id: 1);
        var player2 = SeededPlayer.Generate(id: 2);

        // Create 4 games ordered by StartTime (halfway game will be index 2, which is game3)
        var baseTime = new DateTime(2023, 1, 1, 10, 0, 0);
        var game1 = SeededGame.Generate(
            startTime: baseTime,
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        var game2 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(10),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        var game3 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(20),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        var game4 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(30),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );

        // Add scores to all games
        SeededScore.Generate(game: game1, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game1, player: player2, verificationStatus: VerificationStatus.Verified);

        SeededScore.Generate(game: game2, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game2, player: player2, verificationStatus: VerificationStatus.Verified);

        // Halfway game - player order determines team assignment
        SeededScore.Generate(game: game3, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game3, player: player2, verificationStatus: VerificationStatus.Verified);

        SeededScore.Generate(game: game4, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game4, player: player2, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);

        // Verify team assignments
        foreach (var game in match.Games)
        {
            Assert.Equal(TeamType.TeamVs, game.TeamType);

            var player1Score = game.Scores.First(s => s.Player.Id == player1.Id);
            var player2Score = game.Scores.First(s => s.Player.Id == player2.Id);

            Assert.Equal(Team.Red, player1Score.Team);
            Assert.Equal(Team.Blue, player2Score.Team);
        }
    }

    [Fact]
    public void Check_GivenHalfwayGameWithOnePlayer_AssignsTeamsCorrectly()
    {
        // Arrange
        var tournament = SeededTournament.Generate(teamSize: 1);
        var match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        var player1 = SeededPlayer.Generate(id: 1);
        var player2 = SeededPlayer.Generate(id: 2);

        // Create 2 games ordered by StartTime (halfway game will be index 1, which is game2)
        var baseTime = new DateTime(2023, 1, 1, 10, 0, 0);
        var game1 = SeededGame.Generate(
            startTime: baseTime,
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        var game2 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(10),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );

        // Game 1 with both players
        SeededScore.Generate(game: game1, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game1, player: player2, verificationStatus: VerificationStatus.Verified);

        // Halfway game with only one player (player2)
        SeededScore.Generate(game: game2, player: player2, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);

        // Verify team assignments - player2 should be red (first in halfway game), player1 should be blue
        var game1Player1Score = game1.Scores.First(s => s.Player.Id == player1.Id);
        var game1Player2Score = game1.Scores.First(s => s.Player.Id == player2.Id);
        var game2Player2Score = game2.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Blue, game1Player1Score.Team);
        Assert.Equal(Team.Red, game1Player2Score.Team);
        Assert.Equal(Team.Red, game2Player2Score.Team);
    }

    [Fact]
    public void Check_GivenHalfwayGameWithNoPlayers_UsesOrderedPlayerIds()
    {
        // Arrange
        var tournament = SeededTournament.Generate(teamSize: 1);
        var match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        var player1 = SeededPlayer.Generate(id: 5); // Higher ID
        var player2 = SeededPlayer.Generate(id: 3); // Lower ID

        // Create 2 games ordered by StartTime where halfway game has no scores
        var baseTime = new DateTime(2023, 1, 1, 10, 0, 0);
        var game1 = SeededGame.Generate(
            startTime: baseTime,
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        var game2 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(10),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );

        // Game 1 with both players
        SeededScore.Generate(game: game1, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game1, player: player2, verificationStatus: VerificationStatus.Verified);

        // Halfway game with no scores (empty)
        // No scores added to game2

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);

        // Verify team assignments - lower ID (player2=3) should be red, higher ID (player1=5) should be blue
        var game1Player1Score = game1.Scores.First(s => s.Player.Id == player1.Id);
        var game1Player2Score = game1.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Blue, game1Player1Score.Team);
        Assert.Equal(Team.Red, game1Player2Score.Team);
    }

    #endregion

    #region Conversion Tests

    [Fact]
    public void Check_GivenValidMatchWithTwoPlayersPerGame_ConvertsSuccessfully()
    {
        // Arrange
        var tournament = SeededTournament.Generate(teamSize: 1);
        var match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        var player1 = SeededPlayer.Generate(id: 1);
        var player2 = SeededPlayer.Generate(id: 2);

        var game = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );

        SeededScore.Generate(game: game, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game, player: player2, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);
        Assert.Equal(TeamType.TeamVs, game.TeamType);

        var player1Score = game.Scores.First(s => s.Player.Id == player1.Id);
        var player2Score = game.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Red, player1Score.Team);
        Assert.Equal(Team.Blue, player2Score.Team);
    }

    [Fact]
    public void Check_GivenValidMatchWithOnePlayerPerGame_ConvertsSuccessfully()
    {
        // Arrange
        var tournament = SeededTournament.Generate(teamSize: 1);
        var match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        var player1 = SeededPlayer.Generate(id: 1);
        var player2 = SeededPlayer.Generate(id: 2);

        // Create games ordered by StartTime
        var baseTime = new DateTime(2023, 1, 1, 10, 0, 0);

        // Game with only player1
        var game1 = SeededGame.Generate(
            startTime: baseTime,
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: game1, player: player1, verificationStatus: VerificationStatus.Verified);

        // Game with only player2
        var game2 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(10),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: game2, player: player2, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);

        Assert.Equal(TeamType.TeamVs, game1.TeamType);
        Assert.Equal(TeamType.TeamVs, game2.TeamType);

        var game1Score = game1.Scores.First();
        var game2Score = game2.Scores.First();

        // For 2 games, halfway index is 2/2 = 1, so game2 (index 1) determines assignments
        // Game2 has player2, so player2 becomes red, player1 becomes blue
        Assert.Equal(Team.Blue, game1Score.Team);
        Assert.Equal(Team.Red, game2Score.Team);
    }

    [Fact]
    public void Check_GivenMixedHeadToHeadAndTeamVsGames_OnlyConvertsHeadToHeadGames()
    {
        // Arrange
        var tournament = SeededTournament.Generate(teamSize: 1);
        var match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        var player1 = SeededPlayer.Generate(id: 1);
        var player2 = SeededPlayer.Generate(id: 2);

        // HeadToHead game
        var headToHeadGame = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: headToHeadGame, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: headToHeadGame, player: player2, verificationStatus: VerificationStatus.Verified);

        // TeamVs game (should not be converted)
        var teamVsGame = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: teamVsGame, player: player1, team: Team.Red, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: teamVsGame, player: player2, team: Team.Blue, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);

        // HeadToHead game should be converted
        Assert.Equal(TeamType.TeamVs, headToHeadGame.TeamType);

        // TeamVs game should remain unchanged
        Assert.Equal(TeamType.TeamVs, teamVsGame.TeamType);

        // Verify team assignments for converted game
        var headToHeadPlayer1Score = headToHeadGame.Scores.First(s => s.Player.Id == player1.Id);
        var headToHeadPlayer2Score = headToHeadGame.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Red, headToHeadPlayer1Score.Team);
        Assert.Equal(Team.Blue, headToHeadPlayer2Score.Team);

        // Verify TeamVs game teams remain unchanged
        var teamVsPlayer1Score = teamVsGame.Scores.First(s => s.Player.Id == player1.Id);
        var teamVsPlayer2Score = teamVsGame.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Red, teamVsPlayer1Score.Team);
        Assert.Equal(Team.Blue, teamVsPlayer2Score.Team);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Check_GivenComplexValidMatch_ConvertsAllEligibleGames()
    {
        // Arrange
        var tournament = SeededTournament.Generate(teamSize: 1);
        var match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        var player1 = SeededPlayer.Generate(id: 1);
        var player2 = SeededPlayer.Generate(id: 2);

        // Create multiple HeadToHead games with different score patterns, ordered by StartTime
        var baseTime = new DateTime(2023, 1, 1, 10, 0, 0);

        var game1 = SeededGame.Generate(
            startTime: baseTime,
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: game1, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game1, player: player2, verificationStatus: VerificationStatus.Verified);

        var game2 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(10),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: game2, player: player1, verificationStatus: VerificationStatus.Verified);

        var game3 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(20),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: game3, player: player2, verificationStatus: VerificationStatus.Verified);

        var game4 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(30),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: game4, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game4, player: player2, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);

        // All games should be converted to TeamVs
        Assert.All(match.Games, game => Assert.Equal(TeamType.TeamVs, game.TeamType));

        // Verify team assignments are consistent
        // The halfway game index for 4 games is 4/2 = 2, so game3 (index 2) determines assignments
        // Game3 has only player2, so player2 becomes red, player1 becomes blue
        foreach (var game in match.Games)
        {
            foreach (var score in game.Scores)
            {
                if (score.Player.Id == player1.Id)
                {
                    Assert.Equal(Team.Blue, score.Team);
                }
                else if (score.Player.Id == player2.Id)
                {
                    Assert.Equal(Team.Red, score.Team);
                }
            }
        }
    }

    [Fact]
    public void Check_GivenMatchWithExistingRejectionReasons_PreservesExistingReasons()
    {
        // Arrange
        var tournament = SeededTournament.Generate(teamSize: 1);
        var match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.NoEndTime
        );

        var player1 = SeededPlayer.Generate(id: 1);
        var player2 = SeededPlayer.Generate(id: 2);
        var player3 = SeededPlayer.Generate(id: 3);

        var game = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.NoEndTime
        );

        // Add 2 scores but create a scenario that will fail validation
        SeededScore.Generate(game: game, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game, player: player2, verificationStatus: VerificationStatus.Verified);

        // Add another game with a third player to make total unique players = 3 (which will fail)
        var game2 = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match,
            rejectionReason: GameRejectionReason.None
        );
        SeededScore.Generate(game: game2, player: player3, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.False(result);

        // Should preserve existing rejection reasons and add new ones
        Assert.True(match.RejectionReason.HasFlag(MatchRejectionReason.NoEndTime));
        Assert.True(match.RejectionReason.HasFlag(MatchRejectionReason.FailedTeamVsConversion));

        Assert.True(game.RejectionReason.HasFlag(GameRejectionReason.NoEndTime));
        Assert.True(game.RejectionReason.HasFlag(GameRejectionReason.FailedTeamVsConversion));
    }

    #endregion
}
