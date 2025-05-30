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
        Tournament tournament = SeededTournament.Generate(teamSize: 1);
        Match match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        Game game = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );

        // Add 2 scores (valid count) but with 3 different players across the match
        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);
        Player player3 = SeededPlayer.Generate(id: 3);

        SeededScore.Generate(game: game, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Add another game with the third player to make total unique players = 3
        Game game2 = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
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
        Tournament tournament = SeededTournament.Generate(teamSize: 1);
        Match match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        Game game = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );

        Player player = SeededPlayer.Generate(id: 1);
        SeededScore.Generate(game: game, player: player, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.False(result);
        Assert.True(match.RejectionReason.HasFlag(MatchRejectionReason.FailedTeamVsConversion));
        Assert.True(game.RejectionReason.HasFlag(GameRejectionReason.FailedTeamVsConversion));
    }

    #endregion

    #region Team Assignment Tests

    [Fact]
    public void Check_GivenValidMatch_DeterminesTeamAssignmentsFromHalfwayGame()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(teamSize: 1);
        Match match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // Create 4 games ordered by StartTime (halfway game will be index 2, which is game3)
        var baseTime = new DateTime(2023, 1, 1, 10, 0, 0);
        Game game1 = SeededGame.Generate(
            startTime: baseTime,
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        Game game2 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(10),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        Game game3 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(20),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        Game game4 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(30),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );

        // Add scores to all games
        SeededScore.Generate(game: game1, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game1, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        SeededScore.Generate(game: game2, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game2, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Halfway game - player order determines team assignment
        SeededScore.Generate(game: game3, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game3, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        SeededScore.Generate(game: game4, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game4, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);

        // Verify team assignments
        foreach (Game game in match.Games)
        {
            Assert.Equal(TeamType.TeamVs, game.TeamType);

            GameScore player1Score = game.Scores.First(s => s.Player.Id == player1.Id);
            GameScore player2Score = game.Scores.First(s => s.Player.Id == player2.Id);

            Assert.Equal(Team.Red, player1Score.Team);
            Assert.Equal(Team.Blue, player2Score.Team);
        }
    }

    [Fact]
    public void Check_GivenHalfwayGameWithOnePlayer_AssignsTeamsCorrectly()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(teamSize: 1);
        Match match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // Create 2 games ordered by StartTime (halfway game will be index 1, which is game2)
        var baseTime = new DateTime(2023, 1, 1, 10, 0, 0);
        Game game1 = SeededGame.Generate(
            startTime: baseTime,
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        Game game2 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(10),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );

        // Game 1 with both players
        SeededScore.Generate(game: game1, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game1, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Halfway game with only one player (player2)
        SeededScore.Generate(game: game2, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);

        // Verify team assignments - player2 should be red (first in halfway game), player1 should be blue
        GameScore game1Player1Score = game1.Scores.First(s => s.Player.Id == player1.Id);
        GameScore game1Player2Score = game1.Scores.First(s => s.Player.Id == player2.Id);
        GameScore game2Player2Score = game2.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Blue, game1Player1Score.Team);
        Assert.Equal(Team.Red, game1Player2Score.Team);
        Assert.Equal(Team.Red, game2Player2Score.Team);
    }

    #endregion

    #region Conversion Tests

    [Fact]
    public void Check_GivenValidMatchWithTwoPlayersPerGame_ConvertsSuccessfully()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(teamSize: 1);
        Match match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        Game game = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );

        SeededScore.Generate(game: game, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);
        Assert.Equal(TeamType.TeamVs, game.TeamType);

        GameScore player1Score = game.Scores.First(s => s.Player.Id == player1.Id);
        GameScore player2Score = game.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Red, player1Score.Team);
        Assert.Equal(Team.Blue, player2Score.Team);
    }

    [Fact]
    public void Check_GivenValidMatchWithOnePlayerPerGame_ConvertsSuccessfully()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(teamSize: 1);
        Match match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // Create games ordered by StartTime
        var baseTime = new DateTime(2023, 1, 1, 10, 0, 0);

        // Game with only player1
        Game game1 = SeededGame.Generate(
            startTime: baseTime,
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        SeededScore.Generate(game: game1, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Game with only player2
        Game game2 = SeededGame.Generate(
            startTime: baseTime.AddMinutes(10),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        SeededScore.Generate(game: game2, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);

        Assert.Equal(TeamType.TeamVs, game1.TeamType);
        Assert.Equal(TeamType.TeamVs, game2.TeamType);

        GameScore game1Score = game1.Scores.First();
        GameScore game2Score = game2.Scores.First();

        // For 2 games, halfway index is 2/2 = 1, so game2 (index 1) determines assignments
        // Game2 has player2, so player2 becomes red, player1 becomes blue
        Assert.Equal(Team.Blue, game1Score.Team);
        Assert.Equal(Team.Red, game2Score.Team);
    }

    [Fact]
    public void Check_GivenMixedHeadToHeadAndTeamVsGames_OnlyConvertsHeadToHeadGames()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(teamSize: 1);
        Match match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // HeadToHead game
        Game headToHeadGame = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        SeededScore.Generate(game: headToHeadGame, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: headToHeadGame, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // TeamVs game (should not be converted)
        Game teamVsGame = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
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
        GameScore headToHeadPlayer1Score = headToHeadGame.Scores.First(s => s.Player.Id == player1.Id);
        GameScore headToHeadPlayer2Score = headToHeadGame.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Red, headToHeadPlayer1Score.Team);
        Assert.Equal(Team.Blue, headToHeadPlayer2Score.Team);

        // Verify TeamVs game teams remain unchanged
        GameScore teamVsPlayer1Score = teamVsGame.Scores.First(s => s.Player.Id == player1.Id);
        GameScore teamVsPlayer2Score = teamVsGame.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Red, teamVsPlayer1Score.Team);
        Assert.Equal(Team.Blue, teamVsPlayer2Score.Team);
    }

    #endregion

    #region Rejected Entity Handling Tests

    [Fact]
    public void Check_GivenMatchWithRejectedGamesAndScores_IgnoresRejectedEntitiesCompletely()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(teamSize: 1);
        Match match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);
        Player player3 = SeededPlayer.Generate(id: 3);

        // Create games with explicit StartTime ordering
        var baseTime = new DateTime(2023, 1, 1, 10, 0, 0);

        // Valid HeadToHead game with valid scores
        Game validGame = SeededGame.Generate(
            startTime: baseTime,
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        SeededScore.Generate(game: validGame, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: validGame, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Rejected game that should be completely ignored and not converted
        Game rejectedGame = SeededGame.Generate(
            startTime: baseTime.AddMinutes(10),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.NoEndTime,
            verificationStatus: VerificationStatus.Rejected
        );
        // Add scores to rejected game - these should be completely ignored
        SeededScore.Generate(game: rejectedGame, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: rejectedGame, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Valid game with a rejected score that should be ignored
        Game gameWithRejectedScore = SeededGame.Generate(
            startTime: baseTime.AddMinutes(20),
            teamType: TeamType.TeamVs,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        SeededScore.Generate(game: gameWithRejectedScore, player: player1, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: gameWithRejectedScore, player: player2, verificationStatus: VerificationStatus.Verified);
        // This rejected score should be completely ignored
        SeededScore.Generate(game: gameWithRejectedScore, player: player3, team: Team.NoTeam, verificationStatus: VerificationStatus.Rejected);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result); // Should succeed because rejected entities are completely ignored

        // Verify that only the valid HeadToHead game was converted
        Assert.Equal(TeamType.TeamVs, validGame.TeamType);
        Assert.Equal(TeamType.HeadToHead, rejectedGame.TeamType); // Rejected game should not be converted
        Assert.Equal(TeamType.TeamVs, gameWithRejectedScore.TeamType); // Already TeamVs, should remain unchanged

        // Verify team assignments for the converted game only
        GameScore validGamePlayer1Score = validGame.Scores.First(s => s.Player.Id == player1.Id);
        GameScore validGamePlayer2Score = validGame.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Red, validGamePlayer1Score.Team);
        Assert.Equal(Team.Blue, validGamePlayer2Score.Team);

        // Rejected game scores should not have team assignments
        GameScore rejectedGamePlayer1Score = rejectedGame.Scores.First(s => s.Player.Id == player1.Id);
        GameScore rejectedGamePlayer2Score = rejectedGame.Scores.First(s => s.Player.Id == player2.Id);
        Assert.Equal(Team.NoTeam, rejectedGamePlayer1Score.Team);
        Assert.Equal(Team.NoTeam, rejectedGamePlayer2Score.Team);

        // Verify no rejection reasons were added
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);
        Assert.Equal(GameRejectionReason.None, validGame.RejectionReason);
        Assert.Equal(GameRejectionReason.None, gameWithRejectedScore.RejectionReason);
        // Rejected game should keep its original rejection reason
        Assert.Equal(GameRejectionReason.NoEndTime, rejectedGame.RejectionReason);
    }

    [Fact]
    public void Check_GivenMatchWithRejectedScores_IgnoresRejectedScoresCompletely()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(teamSize: 1);
        Match match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);
        Player player3 = SeededPlayer.Generate(id: 3); // This player will only exist in rejected scores

        var baseTime = new DateTime(2023, 1, 1, 10, 0, 0);

        // Valid HeadToHead game with valid scores from player1 and player2
        Game validGame = SeededGame.Generate(
            startTime: baseTime,
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        SeededScore.Generate(game: validGame, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: validGame, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Another valid game with rejected scores that should be completely ignored
        Game gameWithRejectedScore = SeededGame.Generate(
            startTime: baseTime.AddMinutes(10),
            teamType: TeamType.TeamVs,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        SeededScore.Generate(game: gameWithRejectedScore, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: gameWithRejectedScore, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        // This rejected score should be completely ignored - player3 should not affect processing
        SeededScore.Generate(game: gameWithRejectedScore, player: player3, team: Team.NoTeam, verificationStatus: VerificationStatus.Rejected);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        // Should succeed because rejected scores are completely ignored
        Assert.True(result);

        // Verify that only the HeadToHead game was converted
        Assert.Equal(TeamType.TeamVs, validGame.TeamType);
        Assert.Equal(TeamType.TeamVs, gameWithRejectedScore.TeamType); // Already TeamVs, remains unchanged

        // Verify team assignments for the converted game
        GameScore validGamePlayer1Score = validGame.Scores.First(s => s.Player.Id == player1.Id);
        GameScore validGamePlayer2Score = validGame.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Red, validGamePlayer1Score.Team);
        Assert.Equal(Team.Blue, validGamePlayer2Score.Team);

        // Verify no rejection reasons were added
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);
        Assert.Equal(GameRejectionReason.None, validGame.RejectionReason);
        Assert.Equal(GameRejectionReason.None, gameWithRejectedScore.RejectionReason);
    }

    [Fact]
    public void Check_GivenHeadToHeadGameWithOnlyRejectedScores_IgnoresGameCompletely()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(teamSize: 1);
        Match match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.None
        );

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        var baseTime = new DateTime(2023, 1, 1, 10, 0, 0);

        // Valid HeadToHead game with valid scores
        Game validGame = SeededGame.Generate(
            startTime: baseTime,
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        SeededScore.Generate(game: validGame, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: validGame, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // HeadToHead game with only rejected scores - should be completely ignored
        Game gameWithOnlyRejectedScores = SeededGame.Generate(
            startTime: baseTime.AddMinutes(10),
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
        );
        SeededScore.Generate(game: gameWithOnlyRejectedScores, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Rejected);
        SeededScore.Generate(game: gameWithOnlyRejectedScores, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Rejected);

        // Act
        bool result = AutomationCheck.Check(match);

        // Assert
        Assert.True(result);

        // Only the valid game should be converted
        Assert.Equal(TeamType.TeamVs, validGame.TeamType);
        Assert.Equal(TeamType.HeadToHead, gameWithOnlyRejectedScores.TeamType); // Should not be converted

        // Verify team assignments for the converted game only
        GameScore validGamePlayer1Score = validGame.Scores.First(s => s.Player.Id == player1.Id);
        GameScore validGamePlayer2Score = validGame.Scores.First(s => s.Player.Id == player2.Id);

        Assert.Equal(Team.Red, validGamePlayer1Score.Team);
        Assert.Equal(Team.Blue, validGamePlayer2Score.Team);

        // Game with only rejected scores should not have team assignments
        GameScore rejectedScoreGame1 = gameWithOnlyRejectedScores.Scores.First(s => s.Player.Id == player1.Id);
        GameScore rejectedScoreGame2 = gameWithOnlyRejectedScores.Scores.First(s => s.Player.Id == player2.Id);
        Assert.Equal(Team.NoTeam, rejectedScoreGame1.Team);
        Assert.Equal(Team.NoTeam, rejectedScoreGame2.Team);

        // Verify no rejection reasons were added
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);
        Assert.Equal(GameRejectionReason.None, validGame.RejectionReason);
        Assert.Equal(GameRejectionReason.None, gameWithOnlyRejectedScores.RejectionReason);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Check_GivenMatchWithExistingRejectionReasons_PreservesExistingReasons()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(teamSize: 1);
        Match match = SeededMatch.Generate(
            tournament: tournament,
            rejectionReason: MatchRejectionReason.NoEndTime
        );

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);
        Player player3 = SeededPlayer.Generate(id: 3);

        Game game = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match,
            rejectionReason: GameRejectionReason.NoEndTime,
            verificationStatus: VerificationStatus.Verified
        );

        // Add 2 scores but create a scenario that will fail validation
        SeededScore.Generate(game: game, player: player1, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);
        SeededScore.Generate(game: game, player: player2, team: Team.NoTeam, verificationStatus: VerificationStatus.Verified);

        // Add another game with a third player to make total unique players = 3 (which will fail)
        Game game2 = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match,
            rejectionReason: GameRejectionReason.None,
            verificationStatus: VerificationStatus.Verified
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
