using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.Utilities;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.Stats;

public class RostersHelperTests
{
    [Fact]
    public void GenerateGameRosters_WithValidScores_CreatesCorrectRosters()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        GameScore[] scores =
        [
            SeededScore.Generate(id: 1, score: 600, team: Team.Red, player: SeededPlayer.Generate(id: 1), game: game),
            SeededScore.Generate(id: 2, score: 500, team: Team.Red, player: SeededPlayer.Generate(id: 2), game: game),
            SeededScore.Generate(id: 3, score: 400, team: Team.Red, player: SeededPlayer.Generate(id: 3), game: game),
            SeededScore.Generate(id: 4, score: 300, team: Team.Blue, player: SeededPlayer.Generate(id: 4), game: game),
            SeededScore.Generate(id: 5, score: 200, team: Team.Blue, player: SeededPlayer.Generate(id: 5), game: game),
            SeededScore.Generate(id: 6, score: 100, team: Team.Blue, player: SeededPlayer.Generate(id: 6), game: game)
        ];

        // Act
        ICollection<GameRoster> rosters = RostersHelper.GenerateRosters(scores);

        // Assert
        GameRoster redRoster = rosters.First(r => r.Team == Team.Red);
        GameRoster blueRoster = rosters.First(r => r.Team == Team.Blue);

        Assert.Equal(Team.Red, redRoster.Team);
        Assert.Equal([1, 2, 3], redRoster.Roster);
        Assert.Equal(1500, redRoster.Score);

        Assert.Equal(Team.Blue, blueRoster.Team);
        Assert.Equal([4, 5, 6], blueRoster.Roster);
        Assert.Equal(600, blueRoster.Score);

        Assert.Equal(2, rosters.Count);
    }

    [Fact]
    public void GenerateGameRosters_WithEmptyScores_ReturnsEmptyCollection()
    {
        // Arrange
        GameScore[] scores = [];

        // Act
        ICollection<GameRoster> rosters = RostersHelper.GenerateRosters(scores);

        // Assert
        Assert.Empty(rosters);
    }

    [Fact]
    public void GenerateGameRosters_WithDifferentGameIds_ThrowsException()
    {
        // Arrange
        Game game1 = SeededGame.Generate(id: 1);
        Game game2 = SeededGame.Generate(id: 2);

        GameScore[] scores =
        [
            SeededScore.Generate(id: 1, score: 600, team: Team.Red, player: SeededPlayer.Generate(id: 1), game: game1),
            SeededScore.Generate(id: 2, score: 500, team: Team.Blue, player: SeededPlayer.Generate(id: 2), game: game2)
        ];

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => RostersHelper.GenerateRosters(scores));
    }

    [Fact]
    public void GenerateGameRosters_WithDuplicatePlayersOnSameTeam_DeduplicatesPlayers()
    {
        // Arrange
        Game game = SeededGame.Generate(id: 1);
        Player player = SeededPlayer.Generate(id: 1);

        GameScore[] scores =
        [
            SeededScore.Generate(id: 1, score: 600, team: Team.Red, player: player, game: game),
            SeededScore.Generate(id: 2, score: 500, team: Team.Red, player: player, game: game) // Same player, different score
        ];

        // Act
        ICollection<GameRoster> rosters = RostersHelper.GenerateRosters(scores);

        // Assert
        GameRoster redRoster = rosters.Single();
        Assert.Equal(Team.Red, redRoster.Team);
        Assert.Single(redRoster.Roster);
        Assert.Equal(1, redRoster.Roster[0]);
        Assert.Equal(1100, redRoster.Score); // Sum of both scores
    }

    [Fact]
    public void GenerateMatchRosters_WithEmptyGames_ReturnsEmptyCollection()
    {
        // Arrange
        Game[] games = [];

        // Act
        ICollection<MatchRoster> result = RostersHelper.GenerateRosters(games);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GenerateMatchRosters_WithGamesWithoutRosters_ReturnsEmptyCollection()
    {
        // Arrange
        Game[] games =
        [
            SeededGame.Generate(id: 1),
            SeededGame.Generate(id: 2)
        ];

        foreach (Game game in games)
        {
            Assert.Empty(game.Rosters);
        }

        // Act
        ICollection<MatchRoster> result = RostersHelper.GenerateRosters(games);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GenerateMatchRosters_WithMixedRosters_FiltersEmptyRosters()
    {
        // Arrange
        Game gameWithRosters = SeededGame.Generate(id: 1);
        Game gameWithoutRosters = SeededGame.Generate(id: 2);

        gameWithRosters.Rosters =
        [
            new GameRoster { Team = Team.Red, Roster = [1, 2], Score = 100, GameId = 1 },
            new GameRoster { Team = Team.Blue, Roster = [3, 4], Score = 80, GameId = 1 }
        ];

        Game[] games = [gameWithRosters, gameWithoutRosters];

        // Act
        ICollection<MatchRoster> result = RostersHelper.GenerateRosters(games);

        // Assert
        Assert.Equal(2, result.Count);

        MatchRoster redRoster = result.First(r => r.Team == Team.Red);
        MatchRoster blueRoster = result.First(r => r.Team == Team.Blue);

        Assert.Equal([1, 2], redRoster.Roster);
        Assert.Equal([3, 4], blueRoster.Roster);
        Assert.Equal(0, redRoster.Score); // No wins recorded
        Assert.Equal(0, blueRoster.Score);
    }

    [Fact]
    public void GenerateMatchRosters_WithMultipleGames_CalculatesWinsCorrectly()
    {
        // Arrange
        Game game1 = SeededGame.Generate(id: 1);
        Game game2 = SeededGame.Generate(id: 2);

        // Set up scores for game1 (Red wins)
        game1.Scores =
        [
            SeededScore.Generate(id: 1, score: 600, team: Team.Red, player: SeededPlayer.Generate(id: 1), game: game1),
            SeededScore.Generate(id: 2, score: 400, team: Team.Blue, player: SeededPlayer.Generate(id: 2), game: game1)
        ];

        // Set up scores for game2 (Blue wins)
        game2.Scores =
        [
            SeededScore.Generate(id: 3, score: 300, team: Team.Red, player: SeededPlayer.Generate(id: 1), game: game2),
            SeededScore.Generate(id: 4, score: 500, team: Team.Blue, player: SeededPlayer.Generate(id: 2), game: game2)
        ];

        foreach (Game game in new[] { game1, game2 })
        {
            foreach (GameScore score in game.Scores)
            {
                score.VerificationStatus = VerificationStatus.Verified;
                score.ProcessingStatus = ScoreProcessingStatus.Done;
            }
        }

        Game[] games = [game1, game2];

        // Act
        ICollection<MatchRoster> result = RostersHelper.GenerateRosters(games);

        // Assert
        Assert.Equal(2, result.Count);

        MatchRoster redRoster = result.First(r => r.Team == Team.Red);
        MatchRoster blueRoster = result.First(r => r.Team == Team.Blue);

        Assert.Equal(1, redRoster.Score); // Won 1 game
        Assert.Equal(1, blueRoster.Score); // Won 1 game
    }
}
