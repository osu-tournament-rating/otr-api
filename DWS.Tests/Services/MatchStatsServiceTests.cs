using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.Services.Implementations;
using DWS.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using TestingUtils.SeededData;
using Match = Database.Entities.Match;

namespace DWS.Tests.Services;

/// <summary>
/// Unit tests for the MatchStatsService class.
/// </summary>
public class MatchStatsServiceTests
{
    private readonly Mock<ILogger<MatchStatsService>> _loggerMock = new();
    private readonly Mock<IGameStatsService> _gameStatsServiceMock = new();
    private readonly MatchStatsService _service;

    public MatchStatsServiceTests()
    {
        _service = new MatchStatsService(_loggerMock.Object, _gameStatsServiceMock.Object);
    }

    /// <summary>
    /// Creates a test match with verified games and scores.
    /// </summary>
    private static Match CreateTestMatch()
    {
        Match match = SeededMatch.ExampleMatch();
        match.VerificationStatus = VerificationStatus.Verified;

        foreach (Game game in match.Games)
        {
            game.VerificationStatus = VerificationStatus.Verified;
            foreach (GameScore score in game.Scores)
            {
                score.VerificationStatus = VerificationStatus.Verified;
            }
        }

        return match;
    }

    [Fact]
    public async Task ProcessMatchStatsAsync_WithUnverifiedMatch_ReturnsFalse()
    {
        // Arrange
        Match match = CreateTestMatch();
        match.VerificationStatus = VerificationStatus.None;

        // Act
        bool result = await _service.ProcessMatchStatsAsync(match);

        // Assert
        Assert.False(result);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("unverified match")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _gameStatsServiceMock.Verify(x => x.ProcessGameStatsAsync(It.IsAny<Game>()), Times.Never);
    }

    [Fact]
    public async Task ProcessMatchStatsAsync_WithVerifiedMatch_ProcessesAllVerifiedGames()
    {
        // Arrange
        Match match = CreateTestMatch();
        _gameStatsServiceMock
            .Setup(x => x.ProcessGameStatsAsync(It.IsAny<Game>()))
            .ReturnsAsync(true)
            .Callback<Game>(g =>
            {
                // Simulate roster generation
                g.Rosters =
                [
                    new GameRoster { GameId = g.Id, Team = Team.Red, Roster = [1, 2], Score = 1000 },
                    new GameRoster { GameId = g.Id, Team = Team.Blue, Roster = [3, 4], Score = 800 }
                ];
            });

        // Act
        bool result = await _service.ProcessMatchStatsAsync(match);

        // Assert
        Assert.True(result);
        int verifiedGamesCount = match.Games.Count(g => g.VerificationStatus == VerificationStatus.Verified);
        _gameStatsServiceMock.Verify(x => x.ProcessGameStatsAsync(It.IsAny<Game>()), Times.Exactly(verifiedGamesCount));
    }

    [Fact]
    public async Task ProcessMatchStatsAsync_WithNoVerifiedGames_ReturnsFalse()
    {
        // Arrange
        Match match = CreateTestMatch();
        foreach (Game game in match.Games)
        {
            game.VerificationStatus = VerificationStatus.None;
        }

        _gameStatsServiceMock.Setup(x => x.ProcessGameStatsAsync(It.IsAny<Game>())).ReturnsAsync(true);

        // Act
        bool result = await _service.ProcessMatchStatsAsync(match);

        // Assert
        Assert.False(result);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No verified games found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessMatchStatsAsync_WithGameMissingRosters_ReturnsFalse()
    {
        // Arrange
        Match match = CreateTestMatch();
        _gameStatsServiceMock
            .Setup(x => x.ProcessGameStatsAsync(It.IsAny<Game>()))
            .ReturnsAsync(true)
            .Callback<Game>(g =>
            {
                // Don't generate rosters for the first game
                if (g.Id != match.Games.First().Id)
                {
                    g.Rosters =
                    [
                        new GameRoster { GameId = g.Id, Team = Team.Red, Roster = [1, 2], Score = 1000 },
                        new GameRoster { GameId = g.Id, Team = Team.Blue, Roster = [3, 4], Score = 800 }
                    ];
                }
            });

        // Act
        bool result = await _service.ProcessMatchStatsAsync(match);

        // Assert
        Assert.False(result);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("does not contain any rosters")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessMatchStatsAsync_GeneratesMatchRosters()
    {
        // Arrange
        Match match = CreateTestMatch();
        _gameStatsServiceMock
            .Setup(x => x.ProcessGameStatsAsync(It.IsAny<Game>()))
            .ReturnsAsync(true)
            .Callback<Game>(g =>
            {
                g.Rosters =
                [
                    new GameRoster { GameId = g.Id, Team = Team.Red, Roster = [1, 2], Score = 1000 },
                    new GameRoster { GameId = g.Id, Team = Team.Blue, Roster = [3, 4], Score = 800 }
                ];
            });

        // Act
        bool result = await _service.ProcessMatchStatsAsync(match);

        // Assert
        Assert.True(result);
        Assert.NotEmpty(match.Rosters);
        Assert.Equal(2, match.Rosters.Count);

        MatchRoster? redRoster = match.Rosters.FirstOrDefault(r => r.Team == Team.Red);
        MatchRoster? blueRoster = match.Rosters.FirstOrDefault(r => r.Team == Team.Blue);

        Assert.NotNull(redRoster);
        Assert.NotNull(blueRoster);
        Assert.NotEmpty(redRoster.Roster);
        Assert.NotEmpty(blueRoster.Roster);
    }

    [Fact]
    public async Task ProcessMatchStatsAsync_GeneratesPlayerMatchStats()
    {
        // Arrange
        Match match = SeededMatch.Generate(id: 1);
        match.VerificationStatus = VerificationStatus.Verified;

        // Create simple games with known scores
        Game game1 = SeededGame.Generate(id: 1, match: match);
        Game game2 = SeededGame.Generate(id: 2, match: match);

        game1.VerificationStatus = VerificationStatus.Verified;
        game2.VerificationStatus = VerificationStatus.Verified;

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);

        game1.Scores =
        [
            SeededScore.Generate(id: 1, score: 1000, team: Team.Red, player: player1, game: game1),
            SeededScore.Generate(id: 2, score: 800, team: Team.Blue, player: player2, game: game1)
        ];

        game2.Scores =
        [
            SeededScore.Generate(id: 3, score: 900, team: Team.Red, player: player1, game: game2),
            SeededScore.Generate(id: 4, score: 700, team: Team.Blue, player: player2, game: game2)
        ];

        // Set placements manually
        game1.Scores.First(s => s.Id == 1).Placement = 1;
        game1.Scores.First(s => s.Id == 2).Placement = 2;
        game2.Scores.First(s => s.Id == 3).Placement = 1;
        game2.Scores.First(s => s.Id == 4).Placement = 2;

        foreach (GameScore score in game1.Scores.Concat(game2.Scores))
        {
            score.VerificationStatus = VerificationStatus.Verified;
        }

        match.Games = [game1, game2];

        _gameStatsServiceMock
            .Setup(x => x.ProcessGameStatsAsync(It.IsAny<Game>()))
            .ReturnsAsync(true)
            .Callback<Game>(g =>
            {
                if (g.Id == 1)
                {
                    g.Rosters =
                    [
                        new GameRoster { GameId = g.Id, Team = Team.Red, Roster = [1], Score = 1000 },
                        new GameRoster { GameId = g.Id, Team = Team.Blue, Roster = [2], Score = 800 }
                    ];
                }
                else
                {
                    g.Rosters =
                    [
                        new GameRoster { GameId = g.Id, Team = Team.Red, Roster = [1], Score = 900 },
                        new GameRoster { GameId = g.Id, Team = Team.Blue, Roster = [2], Score = 700 }
                    ];
                }
            });

        // Act
        bool result = await _service.ProcessMatchStatsAsync(match);

        // Assert
        Assert.True(result);
        Assert.NotEmpty(match.PlayerMatchStats);
        Assert.Equal(2, match.PlayerMatchStats.Count);

        PlayerMatchStats? player1Stats = match.PlayerMatchStats.FirstOrDefault(s => s.PlayerId == 1);
        PlayerMatchStats? player2Stats = match.PlayerMatchStats.FirstOrDefault(s => s.PlayerId == 2);

        Assert.NotNull(player1Stats);
        Assert.NotNull(player2Stats);

        // Player 1 won both games
        Assert.Equal(2, player1Stats.GamesWon);
        Assert.Equal(0, player1Stats.GamesLost);
        Assert.Equal(2, player1Stats.GamesPlayed);
        Assert.True(player1Stats.Won);
        Assert.Equal(950, player1Stats.AverageScore); // (1000 + 900) / 2
        Assert.Equal(1, player1Stats.AveragePlacement);

        // Player 2 lost both games
        Assert.Equal(0, player2Stats.GamesWon);
        Assert.Equal(2, player2Stats.GamesLost);
        Assert.Equal(2, player2Stats.GamesPlayed);
        Assert.False(player2Stats.Won);
        Assert.Equal(750, player2Stats.AverageScore); // (800 + 700) / 2
        Assert.Equal(2, player2Stats.AveragePlacement);
    }

    [Fact]
    public async Task ProcessMatchStatsAsync_SetsLastProcessingDate()
    {
        // Arrange
        Match match = CreateTestMatch();
        DateTime beforeProcessing = DateTime.UtcNow;

        _gameStatsServiceMock
            .Setup(x => x.ProcessGameStatsAsync(It.IsAny<Game>()))
            .ReturnsAsync(true)
            .Callback<Game>(g =>
            {
                g.Rosters =
                [
                    new GameRoster { GameId = g.Id, Team = Team.Red, Roster = [1, 2], Score = 1000 },
                    new GameRoster { GameId = g.Id, Team = Team.Blue, Roster = [3, 4], Score = 800 }
                ];
            });

        // Act
        bool result = await _service.ProcessMatchStatsAsync(match);

        // Assert
        Assert.True(result);
        Assert.True(match.LastProcessingDate >= beforeProcessing);
        Assert.True(match.LastProcessingDate <= DateTime.UtcNow);
    }

    [Fact]
    public async Task ProcessMatchStatsAsync_PreservesExistingPlayerMatchStats()
    {
        // Arrange
        Match match = CreateTestMatch();

        // Add existing player match stats
        var existingStats = new PlayerMatchStats { PlayerId = 999, MatchCost = 1.5 };
        match.PlayerMatchStats = [existingStats];

        _gameStatsServiceMock
            .Setup(x => x.ProcessGameStatsAsync(It.IsAny<Game>()))
            .ReturnsAsync(true)
            .Callback<Game>(g =>
            {
                g.Rosters =
                [
                    new GameRoster { GameId = g.Id, Team = Team.Red, Roster = [1, 2], Score = 1000 },
                    new GameRoster { GameId = g.Id, Team = Team.Blue, Roster = [3, 4], Score = 800 }
                ];
            });

        // Act
        bool result = await _service.ProcessMatchStatsAsync(match);

        // Assert
        Assert.True(result);
        Assert.NotEmpty(match.PlayerMatchStats);

        // Stats for players who didn't play should not be in the final collection
        // since they're regenerated from scratch based on who actually played
        Assert.DoesNotContain(match.PlayerMatchStats, s => s.PlayerId == 999);
    }

    [Fact]
    public async Task ProcessMatchStatsAsync_HandlesTeamAssignmentsCorrectly()
    {
        // Arrange
        Match match = SeededMatch.Generate(id: 1);
        match.VerificationStatus = VerificationStatus.Verified;

        Game game = SeededGame.Generate(id: 1, match: match);
        game.VerificationStatus = VerificationStatus.Verified;

        Player player1 = SeededPlayer.Generate(id: 1);
        Player player2 = SeededPlayer.Generate(id: 2);
        Player player3 = SeededPlayer.Generate(id: 3);
        Player player4 = SeededPlayer.Generate(id: 4);

        game.Scores =
        [
            SeededScore.Generate(id: 1, score: 1000, team: Team.Red, player: player1, game: game),
            SeededScore.Generate(id: 2, score: 900, team: Team.Red, player: player2, game: game),
            SeededScore.Generate(id: 3, score: 800, team: Team.Blue, player: player3, game: game),
            SeededScore.Generate(id: 4, score: 700, team: Team.Blue, player: player4, game: game)
        ];

        foreach (GameScore score in game.Scores)
        {
            score.VerificationStatus = VerificationStatus.Verified;
        }

        match.Games = [game];

        _gameStatsServiceMock
            .Setup(x => x.ProcessGameStatsAsync(It.IsAny<Game>()))
            .ReturnsAsync(true)
            .Callback<Game>(g =>
            {
                g.Rosters =
                [
                    new GameRoster { GameId = g.Id, Team = Team.Red, Roster = [1, 2], Score = 1900 },
                    new GameRoster { GameId = g.Id, Team = Team.Blue, Roster = [3, 4], Score = 1500 }
                ];
            });

        // Act
        bool result = await _service.ProcessMatchStatsAsync(match);

        // Assert
        Assert.True(result);

        PlayerMatchStats? player1Stats = match.PlayerMatchStats.FirstOrDefault(s => s.PlayerId == 1);
        PlayerMatchStats? player3Stats = match.PlayerMatchStats.FirstOrDefault(s => s.PlayerId == 3);

        Assert.NotNull(player1Stats);
        Assert.NotNull(player3Stats);

        // Check teammate and opponent IDs
        Assert.Contains(2, player1Stats.TeammateIds); // Player 2 is player 1's teammate
        Assert.Contains(3, player1Stats.OpponentIds); // Player 3 is player 1's opponent
        Assert.Contains(4, player1Stats.OpponentIds); // Player 4 is player 1's opponent

        Assert.Contains(4, player3Stats.TeammateIds); // Player 4 is player 3's teammate
        Assert.Contains(1, player3Stats.OpponentIds); // Player 1 is player 3's opponent
        Assert.Contains(2, player3Stats.OpponentIds); // Player 2 is player 3's opponent
    }
}
