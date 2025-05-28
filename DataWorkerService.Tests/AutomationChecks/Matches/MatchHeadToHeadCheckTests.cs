using Common.Enums;
using Common.Enums.Verification;
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
        bool actualPass = AutomationCheck.Check(match);

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
        match.Tournament.LobbySize = tournamentLobbySize;

        SeededGame.Generate(match: match);

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
    }

    [Theory]
    // TeamType not HeadToHead
    [InlineData(TeamType.TagCoop, 2)]
    [InlineData(TeamType.TeamVs, 2)]
    // Score count not 2
    [InlineData(TeamType.HeadToHead, 1)]
    [InlineData(TeamType.HeadToHead, 3)]
    public void Check_GivenGamesIneligibleForConversion_PassesWithNoRejectionReason(
        TeamType gameTeamType,
        int gameScoreCount
    )
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbySize = 1;

        foreach (int idx in Enumerable.Range(0, 2))
        {
            Game game = SeededGame.Generate(
                teamType: gameTeamType,
                match: match
            );

            if (gameScoreCount <= 0)
            {
                continue;
            }

            foreach (int _ in Enumerable.Range(1, gameScoreCount))
            {
                // Set differing player ids to ensure the check fails later
                // This test is only meant to test the eligibility constraints
                SeededScore.Generate(
                    team: Team.NoTeam,
                    player: SeededPlayer.Generate(id: idx),
                    game: game
                );
            }
        }

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);
    }

    [Fact]
    public void Check_GivenGamesEligibleForConversion_WithMoreThanTwoUniquePlayers_FailsAllWithFailedTeamVsConversion()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbySize = 1;

        // Creates 2 games, 2 scores each, with players having ids of 1, 2, 3, and 4
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            team: Team.NoTeam,
            player: SeededPlayer.Generate(id: 1),
            game: gameOne
        );
        SeededScore.Generate(
            team: Team.NoTeam, player:
            SeededPlayer.Generate(id: 2),
            game: gameOne
        );

        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            team: Team.NoTeam,
            player: SeededPlayer.Generate(id: 3),
            game: gameTwo
        );
        SeededScore.Generate(
            team: Team.NoTeam, player:
            SeededPlayer.Generate(id: 4),
            game: gameTwo
        );

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.False(actualPass);
        Assert.Equal(MatchRejectionReason.FailedTeamVsConversion, match.RejectionReason);
        Assert.True(match.Games.All(g => g.RejectionReason.HasFlag(GameRejectionReason.FailedTeamVsConversion)));
    }

    [Fact]
    public void Check_GivenGamesEligibleForConversion_WithDifferentPlayersInDifferentGames_FailsAllWithFailedTeamVsConversion()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbySize = 1;

        // Creates 2 games with different pairs of players
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            team: Team.NoTeam,
            player: SeededPlayer.Generate(id: 1),
            game: gameOne
        );
        SeededScore.Generate(
            team: Team.NoTeam,
            player: SeededPlayer.Generate(id: 2),
            game: gameOne
        );

        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            team: Team.NoTeam,
            player: SeededPlayer.Generate(id: 1), // Player 1 is consistent
            game: gameTwo
        );
        SeededScore.Generate(
            team: Team.NoTeam,
            player: SeededPlayer.Generate(id: 3), // Different player (3 instead of 2)
            game: gameTwo
        );

        // Act
        bool actualPass = AutomationCheck.Check(match);

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
        match.Tournament.LobbySize = 1;

        // Creates 2 games, 2 scores each, with players having ids of 1 and 2
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            team: Team.NoTeam,
            player: SeededPlayer.Generate(id: 1),
            game: gameOne
        );
        SeededScore.Generate(
            team: Team.NoTeam, player:
            SeededPlayer.Generate(id: 2),
            game: gameOne
        );

        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            team: Team.NoTeam,
            player: SeededPlayer.Generate(id: 1),
            game: gameTwo
        );
        SeededScore.Generate(
            team: Team.NoTeam, player:
            SeededPlayer.Generate(id: 2),
            game: gameTwo
        );

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);

        Assert.True(match.Games.All(g => g.TeamType is TeamType.TeamVs));
        Assert.True(match.Games.SelectMany(g => g.Scores).All(s => s.Team is Team.Blue or Team.Red));
    }

    [Fact]
    public void Check_AssignsTeamRedToPlayerWithMostPoints_WhenConvertingToTeamVs()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbySize = 1;

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // Game 1: Player 1 scores 1000, Player 2 scores 500
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            score: 1000,
            team: Team.NoTeam,
            player: player1,
            game: gameOne
        );
        SeededScore.Generate(
            score: 500,
            team: Team.NoTeam,
            player: player2,
            game: gameOne
        );

        // Game 2: Player 1 scores 800, Player 2 scores 900
        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            score: 800,
            team: Team.NoTeam,
            player: player1,
            game: gameTwo
        );
        SeededScore.Generate(
            score: 900,
            team: Team.NoTeam,
            player: player2,
            game: gameTwo
        );

        // Total: Player 1 = 1800, Player 2 = 1400
        // Player 1 should get Team Red

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);

        // Verify all games are converted to TeamVs
        Assert.True(match.Games.All(g => g.TeamType is TeamType.TeamVs));

        // Verify Player 1 (highest total score) gets Team Red
        IEnumerable<GameScore> player1Scores = match.Games.SelectMany(g => g.Scores).Where(s => s.Player.Id == 1);
        IEnumerable<GameScore> player2Scores = match.Games.SelectMany(g => g.Scores).Where(s => s.Player.Id == 2);

        Assert.True(player1Scores.All(s => s.Team == Team.Red));
        Assert.True(player2Scores.All(s => s.Team == Team.Blue));
    }

    [Fact]
    public void Check_AssignsTeamRedToPlayerWithMostPoints_WhenSecondPlayerHasHigherTotal()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbySize = 1;

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // Game 1: Player 1 scores 500, Player 2 scores 1000
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            score: 500,
            team: Team.NoTeam,
            player: player1,
            game: gameOne
        );
        SeededScore.Generate(
            score: 1000,
            team: Team.NoTeam,
            player: player2,
            game: gameOne
        );

        // Game 2: Player 1 scores 600, Player 2 scores 800
        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            score: 600,
            team: Team.NoTeam,
            player: player1,
            game: gameTwo
        );
        SeededScore.Generate(
            score: 800,
            team: Team.NoTeam,
            player: player2,
            game: gameTwo
        );

        // Total: Player 1 = 1100, Player 2 = 1800
        // Player 2 should get Team Red

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);

        // Verify all games are converted to TeamVs
        Assert.True(match.Games.All(g => g.TeamType is TeamType.TeamVs));

        // Verify Player 2 (highest total score) gets Team Red
        IEnumerable<GameScore> player1Scores = match.Games.SelectMany(g => g.Scores).Where(s => s.Player.Id == 1);
        IEnumerable<GameScore> player2Scores = match.Games.SelectMany(g => g.Scores).Where(s => s.Player.Id == 2);

        Assert.True(player1Scores.All(s => s.Team == Team.Blue));
        Assert.True(player2Scores.All(s => s.Team == Team.Red));
    }

    [Fact]
    public void Check_HandlesEqualTotalScores_AssignsTeamRedToFirstPlayerInOrderByDescending()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbySize = 1;

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // Game 1: Player 1 scores 1000, Player 2 scores 500
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            score: 1000,
            team: Team.NoTeam,
            player: player1,
            game: gameOne
        );
        SeededScore.Generate(
            score: 500,
            team: Team.NoTeam,
            player: player2,
            game: gameOne
        );

        // Game 2: Player 1 scores 500, Player 2 scores 1000
        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            score: 500,
            team: Team.NoTeam,
            player: player1,
            game: gameTwo
        );
        SeededScore.Generate(
            score: 1000,
            team: Team.NoTeam,
            player: player2,
            game: gameTwo
        );

        // Total: Player 1 = 1500, Player 2 = 1500 (equal)
        // The first player returned by OrderByDescending should get Team Red

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);

        // Verify all games are converted to TeamVs
        Assert.True(match.Games.All(g => g.TeamType is TeamType.TeamVs));

        // Verify one player gets Red and the other gets Blue (deterministic based on OrderByDescending)
        IEnumerable<GameScore> player1Scores = match.Games.SelectMany(g => g.Scores).Where(s => s.Player.Id == 1);
        IEnumerable<GameScore> player2Scores = match.Games.SelectMany(g => g.Scores).Where(s => s.Player.Id == 2);

        List<Team> player1Teams = player1Scores.Select(s => s.Team).Distinct().ToList();
        List<Team> player2Teams = player2Scores.Select(s => s.Team).Distinct().ToList();

        // Each player should have exactly one team assigned
        Assert.Single(player1Teams);
        Assert.Single(player2Teams);

        // Teams should be different
        Assert.NotEqual(player1Teams.First(), player2Teams.First());

        // One should be Red, one should be Blue
        Assert.Contains(Team.Red, new[] { player1Teams.First(), player2Teams.First() });
        Assert.Contains(Team.Blue, new[] { player1Teams.First(), player2Teams.First() });
    }

    [Fact]
    public void Check_WorksWithMixedGames_OnlyConvertsHeadToHeadGames()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbySize = 1;

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // Game 1: HeadToHead game
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            score: 1000,
            team: Team.NoTeam,
            player: player1,
            game: gameOne
        );
        SeededScore.Generate(
            score: 500,
            team: Team.NoTeam,
            player: player2,
            game: gameOne
        );

        // Game 2: Already TeamVs game (should not be affected)
        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match
        );
        SeededScore.Generate(
            score: 800,
            team: Team.Red,
            player: player1,
            game: gameTwo
        );
        SeededScore.Generate(
            score: 900,
            team: Team.Blue,
            player: player2,
            game: gameTwo
        );

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);

        // Verify only the HeadToHead game was converted
        Assert.Equal(TeamType.TeamVs, gameOne.TeamType);
        Assert.Equal(TeamType.TeamVs, gameTwo.TeamType); // Should remain unchanged

        // Verify the HeadToHead game got proper team assignments
        List<GameScore> gameOneScores = gameOne.Scores.ToList();
        Assert.True(gameOneScores.All(s => s.Team is Team.Blue or Team.Red));

        // Verify the already TeamVs game was not affected
        List<GameScore> gameTwoScores = gameTwo.Scores.ToList();
        Assert.Equal(Team.Red, gameTwoScores.First(s => s.Player.Id == 1).Team);
        Assert.Equal(Team.Blue, gameTwoScores.First(s => s.Player.Id == 2).Team);
    }

    [Fact]
    public void Check_HandlesHeadToHeadInMiddleOfTeamVsGames_ConvertsCorrectly()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbySize = 1;

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // Game 1: TeamVs game
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match
        );
        SeededScore.Generate(
            score: 1000,
            team: Team.Red,
            player: player1,
            game: gameOne
        );
        SeededScore.Generate(
            score: 500,
            team: Team.Blue,
            player: player2,
            game: gameOne
        );

        // Game 2: HeadToHead game in the middle
        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            score: 800,
            team: Team.NoTeam,
            player: player1,
            game: gameTwo
        );
        SeededScore.Generate(
            score: 900,
            team: Team.NoTeam,
            player: player2,
            game: gameTwo
        );

        // Game 3: Another TeamVs game
        Game gameThree = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match
        );
        SeededScore.Generate(
            score: 600,
            team: Team.Red,
            player: player1,
            game: gameThree
        );
        SeededScore.Generate(
            score: 700,
            team: Team.Blue,
            player: player2,
            game: gameThree
        );

        // Total scores: Player 1 = 2400, Player 2 = 2100
        // Player 1 should get Team Red in the converted HeadToHead game

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);

        // Verify all games are TeamVs after processing
        Assert.True(match.Games.All(g => g.TeamType == TeamType.TeamVs));

        // Verify the original TeamVs games were not affected
        List<GameScore> gameOneScores = gameOne.Scores.ToList();
        Assert.Equal(Team.Red, gameOneScores.First(s => s.Player.Id == 1).Team);
        Assert.Equal(Team.Blue, gameOneScores.First(s => s.Player.Id == 2).Team);

        List<GameScore> gameThreeScores = gameThree.Scores.ToList();
        Assert.Equal(Team.Red, gameThreeScores.First(s => s.Player.Id == 1).Team);
        Assert.Equal(Team.Blue, gameThreeScores.First(s => s.Player.Id == 2).Team);

        // Verify the HeadToHead game was converted with correct team assignment
        // Player 1 has highest total score (2400) so should get Team Red
        List<GameScore> gameTwoScores = gameTwo.Scores.ToList();
        Assert.Equal(Team.Red, gameTwoScores.First(s => s.Player.Id == 1).Team);
        Assert.Equal(Team.Blue, gameTwoScores.First(s => s.Player.Id == 2).Team);
    }

    [Fact]
    public void Check_HandlesConflictingTeamAssignments_UsesTotalScoreForConsistency()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbySize = 1;

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // Game 1: TeamVs game where Player 1 is Blue, Player 2 is Red
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match
        );
        SeededScore.Generate(
            score: 500,
            team: Team.Blue,  // Player 1 is Blue
            player: player1,
            game: gameOne
        );
        SeededScore.Generate(
            score: 1000,
            team: Team.Red,   // Player 2 is Red
            player: player2,
            game: gameOne
        );

        // Game 2: Another TeamVs game where Player 1 is Blue, Player 2 is Red
        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match
        );
        SeededScore.Generate(
            score: 600,
            team: Team.Blue,  // Player 1 is Blue again
            player: player1,
            game: gameTwo
        );
        SeededScore.Generate(
            score: 700,
            team: Team.Red,   // Player 2 is Red again
            player: player2,
            game: gameTwo
        );

        // Game 3: TeamVs game where Player 1 is Red, Player 2 is Blue (minority case)
        Game gameThree = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match
        );
        SeededScore.Generate(
            score: 800,
            team: Team.Red,   // Player 1 is Red (minority)
            player: player1,
            game: gameThree
        );
        SeededScore.Generate(
            score: 900,
            team: Team.Blue,  // Player 2 is Blue (minority)
            player: player2,
            game: gameThree
        );

        // Game 4: HeadToHead game (will be converted)
        // Player 1 has been Blue in 2/3 games, Red in 1/3 games - should get Blue
        // Player 2 has been Red in 2/3 games, Blue in 1/3 games - should get Red
        Game gameFour = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            score: 1000,
            team: Team.NoTeam,
            player: player1,
            game: gameFour
        );
        SeededScore.Generate(
            score: 1100,
            team: Team.NoTeam,
            player: player2,
            game: gameFour
        );

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);

        // Verify all games are TeamVs after processing
        Assert.True(match.Games.All(g => g.TeamType == TeamType.TeamVs));

        // Verify the original TeamVs games were NOT affected (preserves original team assignments)
        List<GameScore> gameOneScores = gameOne.Scores.ToList();
        Assert.Equal(Team.Blue, gameOneScores.First(s => s.Player.Id == 1).Team);
        Assert.Equal(Team.Red, gameOneScores.First(s => s.Player.Id == 2).Team);

        List<GameScore> gameTwoScores = gameTwo.Scores.ToList();
        Assert.Equal(Team.Blue, gameTwoScores.First(s => s.Player.Id == 1).Team);
        Assert.Equal(Team.Red, gameTwoScores.First(s => s.Player.Id == 2).Team);

        List<GameScore> gameThreeScores = gameThree.Scores.ToList();
        Assert.Equal(Team.Red, gameThreeScores.First(s => s.Player.Id == 1).Team);
        Assert.Equal(Team.Blue, gameThreeScores.First(s => s.Player.Id == 2).Team);

        // Verify the HeadToHead game was converted based on most common team colors
        // Player 1 was Blue in 2/3 games, so should get Blue in the converted game
        // Player 2 was Red in 2/3 games, so should get Red in the converted game
        List<GameScore> gameFourScores = gameFour.Scores.ToList();
        Assert.Equal(Team.Blue, gameFourScores.First(s => s.Player.Id == 1).Team);
        Assert.Equal(Team.Red, gameFourScores.First(s => s.Player.Id == 2).Team);
    }

    [Fact]
    public void Check_UsesMostCommonTeamColors_WithTieBreakingFromSecondToLastGame()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbySize = 1;

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // Game 1: Player 1 is Red, Player 2 is Blue
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match
        );
        SeededScore.Generate(
            score: 500,
            team: Team.Red,
            player: player1,
            game: gameOne
        );
        SeededScore.Generate(
            score: 600,
            team: Team.Blue,
            player: player2,
            game: gameOne
        );

        // Game 2: Player 1 is Blue, Player 2 is Red (creates a tie - each player has 1 Red, 1 Blue)
        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match
        );
        SeededScore.Generate(
            score: 700,
            team: Team.Blue,  // This is the second-to-last game assignment for Player 1
            player: player1,
            game: gameTwo
        );
        SeededScore.Generate(
            score: 800,
            team: Team.Red,   // This is the second-to-last game assignment for Player 2
            player: player2,
            game: gameTwo
        );

        // Game 3: HeadToHead game (will be converted)
        // Since there's a tie (1-1 for each player), should use second-to-last game (Game 2)
        // Player 1 was Blue in Game 2, so should get Blue
        // Player 2 was Red in Game 2, so should get Red
        Game gameThree = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            score: 900,
            team: Team.NoTeam,
            player: player1,
            game: gameThree
        );
        SeededScore.Generate(
            score: 1000,
            team: Team.NoTeam,
            player: player2,
            game: gameThree
        );

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);

        // Verify all games are TeamVs after processing
        Assert.True(match.Games.All(g => g.TeamType == TeamType.TeamVs));

        // Verify the HeadToHead game was converted using second-to-last game's team assignment
        List<GameScore> gameThreeScores = gameThree.Scores.ToList();
        Assert.Equal(Team.Blue, gameThreeScores.First(s => s.Player.Id == 1).Team);
        Assert.Equal(Team.Red, gameThreeScores.First(s => s.Player.Id == 2).Team);
    }

    [Fact]
    public void Check_FailsConversion_WhenCannotDetermineConsistentTeamAssignment()
    {
        // Arrange
        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None);
        match.Tournament.LobbySize = 1;

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        // Game 1: Only one TeamVs game with Player 1 Red, Player 2 Blue
        // This creates a tie (1-0 vs 0-1) but there's no second-to-last game to break the tie
        Game gameOne = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            match: match
        );
        SeededScore.Generate(
            score: 500,
            team: Team.Red,
            player: player1,
            game: gameOne
        );
        SeededScore.Generate(
            score: 600,
            team: Team.Blue,
            player: player2,
            game: gameOne
        );

        // Game 2: HeadToHead game (conversion should fail)
        Game gameTwo = SeededGame.Generate(
            teamType: TeamType.HeadToHead,
            match: match
        );
        SeededScore.Generate(
            score: 700,
            team: Team.NoTeam,
            player: player1,
            game: gameTwo
        );
        SeededScore.Generate(
            score: 800,
            team: Team.NoTeam,
            player: player2,
            game: gameTwo
        );

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.False(actualPass);
        Assert.Equal(MatchRejectionReason.FailedTeamVsConversion, match.RejectionReason);
        Assert.True(match.Games.Where(g => g.TeamType == TeamType.HeadToHead)
            .All(g => g.RejectionReason.HasFlag(GameRejectionReason.FailedTeamVsConversion)));
    }
}
