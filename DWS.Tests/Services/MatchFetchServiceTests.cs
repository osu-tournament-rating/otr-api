using System.Reflection;
using Common.Enums;
using Database.Entities;
using DWS.Services;
using OsuApiClient.Domain.Osu.Multiplayer;
using ApiGameScore = OsuApiClient.Domain.Osu.Multiplayer.GameScore;

namespace DWS.Tests.Services;

public class MatchFetchServiceTests
{
    [Fact]
    public void CreateScoreFromApi_WhenScoreHasEasyMod_AppliesMultiplier()
    {
        // Arrange
        var game = new Game { Id = 1, Ruleset = Ruleset.Osu };
        var player = new Player { Id = 1 };
        var apiScore = new ApiGameScore
        {
            UserId = 12345,
            Score = 100000,
            MaxCombo = 150,
            Passed = true,
            Perfect = false,
            Grade = ScoreGrade.A,
            Mods = Mods.Easy,
            SlotInfo = new GameSlotInfo { Team = Team.Blue },
            Statistics = new GameScoreStatistics
            {
                Count300 = 100,
                Count100 = 50,
                Count50 = 10,
                CountMiss = 5,
                CountGeki = 1,
                CountKatu = 2
            }
        };

        // Use reflection to access the private method
        MethodInfo? methodInfo = typeof(MatchFetchService).GetMethod("CreateScoreFromApi",
            BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (Database.Entities.GameScore)methodInfo!.Invoke(null, [game, player, apiScore])!;

        // Assert
        Assert.Equal(175000, result.Score); // 100000 * 1.75
        Assert.Equal(ScoreGrade.A, result.Grade);
        Assert.False(result.Perfect);
        Assert.True(result.Mods.HasFlag(Mods.Easy));
    }

    [Fact]
    public void CreateScoreFromApi_WhenScoreHasNoEasyMod_UsesOriginalScore()
    {
        // Arrange
        var game = new Game { Id = 1, Ruleset = Ruleset.Osu };
        var player = new Player { Id = 1 };
        var apiScore = new ApiGameScore
        {
            UserId = 12345,
            Score = 200000,
            MaxCombo = 250,
            Passed = true,
            Perfect = true,
            Grade = ScoreGrade.S,
            Mods = Mods.Hidden | Mods.DoubleTime,
            SlotInfo = new GameSlotInfo { Team = Team.Red },
            Statistics = new GameScoreStatistics
            {
                Count300 = 200,
                Count100 = 10,
                Count50 = 0,
                CountMiss = 0,
                CountGeki = 5,
                CountKatu = 0
            }
        };

        // Use reflection to access the private method
        var methodInfo = typeof(MatchFetchService).GetMethod("CreateScoreFromApi",
            BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (Database.Entities.GameScore)methodInfo!.Invoke(null, [game, player, apiScore])!;

        // Assert
        Assert.Equal(200000, result.Score); // No multiplier
        Assert.Equal(ScoreGrade.S, result.Grade);
        Assert.True(result.Perfect);
        Assert.True(result.Mods.HasFlag(Mods.Hidden));
        Assert.True(result.Mods.HasFlag(Mods.DoubleTime));
        Assert.False(result.Mods.HasFlag(Mods.Easy));
    }

    [Fact]
    public void UpdateScoreFromApi_WhenScoreHasEasyMod_AppliesMultiplier()
    {
        // Arrange
        var existingScore = new Database.Entities.GameScore
        {
            Score = 50000,
            MaxCombo = 100,
            Count300 = 50,
            Count100 = 25,
            Count50 = 5,
            CountMiss = 2,
            CountGeki = 0,
            CountKatu = 0,
            Pass = true,
            Perfect = false,
            Grade = ScoreGrade.C,
            Mods = Mods.None
        };

        var apiScore = new ApiGameScore
        {
            UserId = 12345,
            Score = 150000,
            MaxCombo = 300,
            Passed = true,
            Perfect = false,
            Grade = ScoreGrade.B,
            Mods = Mods.Easy,
            SlotInfo = new GameSlotInfo { Team = Team.Blue },
            Statistics = new GameScoreStatistics
            {
                Count300 = 150,
                Count100 = 75,
                Count50 = 20,
                CountMiss = 10,
                CountGeki = 3,
                CountKatu = 1
            }
        };

        // Use reflection to access the private method
        MethodInfo? methodInfo = typeof(MatchFetchService).GetMethod("UpdateScoreFromApi",
            BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        methodInfo!.Invoke(null, [existingScore, apiScore]);

        // Assert
        Assert.Equal(262500, existingScore.Score); // 150000 * 1.75
        Assert.Equal(300, existingScore.MaxCombo);
        Assert.Equal(150, existingScore.Count300);
        Assert.Equal(75, existingScore.Count100);
        Assert.Equal(20, existingScore.Count50);
        Assert.Equal(10, existingScore.CountMiss);
        Assert.Equal(3, existingScore.CountGeki);
        Assert.Equal(1, existingScore.CountKatu);
        Assert.True(existingScore.Pass);
        Assert.False(existingScore.Perfect);
        Assert.Equal(ScoreGrade.B, existingScore.Grade);
        Assert.True(existingScore.Mods.HasFlag(Mods.Easy));
    }

    [Fact]
    public void CreateScoreFromApi_MapsAllFieldsCorrectly()
    {
        // Arrange
        var game = new Game { Id = 1, Ruleset = Ruleset.ManiaOther };
        var player = new Player { Id = 2 };
        var apiScore = new ApiGameScore
        {
            UserId = 54321,
            Score = 987654,
            MaxCombo = 456,
            Passed = true,
            Perfect = true,
            Grade = ScoreGrade.SS,
            Mods = Mods.HardRock | Mods.Hidden,
            SlotInfo = new GameSlotInfo { Team = Team.Red },
            Statistics = new GameScoreStatistics
            {
                Count300 = 300,
                Count100 = 100,
                Count50 = 50,
                CountMiss = 0,
                CountGeki = 10,
                CountKatu = 5
            }
        };

        // Use reflection to access the private method
        var methodInfo = typeof(MatchFetchService).GetMethod("CreateScoreFromApi",
            BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (Database.Entities.GameScore)methodInfo!.Invoke(null, [game, player, apiScore])!;

        // Assert
        Assert.Equal(game.Id, result.GameId);
        Assert.Equal(player.Id, result.PlayerId);
        Assert.Equal(Team.Red, result.Team);
        Assert.Equal(987654, result.Score);
        Assert.Equal(456, result.MaxCombo);
        Assert.Equal(300, result.Count300);
        Assert.Equal(100, result.Count100);
        Assert.Equal(50, result.Count50);
        Assert.Equal(0, result.CountMiss);
        Assert.Equal(10, result.CountGeki);
        Assert.Equal(5, result.CountKatu);
        Assert.True(result.Pass);
        Assert.True(result.Perfect);
        Assert.Equal(ScoreGrade.SS, result.Grade);
        Assert.True(result.Mods.HasFlag(Mods.HardRock));
        Assert.True(result.Mods.HasFlag(Mods.Hidden));
        Assert.Equal(Ruleset.ManiaOther, result.Ruleset);
    }
}
