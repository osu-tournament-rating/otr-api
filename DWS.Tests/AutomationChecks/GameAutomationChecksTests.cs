
using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.AutomationChecks;
using Microsoft.Extensions.Logging;
using Moq;
using TestingUtils.SeededData;
using Match = Database.Entities.Match;

namespace DWS.Tests.AutomationChecks;

public class GameAutomationChecksTests
{
    private readonly Mock<ILogger<GameAutomationChecks>> _loggerMock = new();
    private readonly GameAutomationChecks _checker;

    public GameAutomationChecksTests()
    {
        _checker = new GameAutomationChecks(_loggerMock.Object);
    }


    [Theory]
    [InlineData(TeamType.TeamVs, GameRejectionReason.None)]
    [InlineData(TeamType.HeadToHead, GameRejectionReason.InvalidTeamType)]
    [InlineData(TeamType.TagCoop, GameRejectionReason.InvalidTeamType)]
    [InlineData(TeamType.TagTeamVs, GameRejectionReason.InvalidTeamType)]
    public void Process_TeamTypeCheck(TeamType teamType, GameRejectionReason expected)
    {
        Game game = SeededGame.Generate(
            teamType: teamType,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);
        Assert.True(result.HasFlag(expected));
    }

    [Theory]
    [InlineData(ScoringType.ScoreV2, GameRejectionReason.None)]
    [InlineData(ScoringType.Score, GameRejectionReason.InvalidScoringType)]
    [InlineData(ScoringType.Accuracy, GameRejectionReason.InvalidScoringType)]
    [InlineData(ScoringType.Combo, GameRejectionReason.InvalidScoringType)]
    public void Process_ScoringTypeCheck(ScoringType scoringType, GameRejectionReason expected)
    {
        Game game = SeededGame.Generate(
            scoringType: scoringType,
            teamType: TeamType.TeamVs,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);
        Assert.True(result.HasFlag(expected));
    }

    [Fact]
    public void Process_ScoreCountCheck_NoScores()
    {
        Game game = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;
        game.Scores = new List<GameScore>();
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);
        Assert.True(result.HasFlag(GameRejectionReason.NoScores));
    }

    [Fact]
    public void Process_ScoreCountCheck_NoValidScores()
    {
        Game game = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;
        game.Scores = new List<GameScore>
        {
            SeededScore.Generate(verificationStatus: VerificationStatus.Rejected, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.PreRejected, mods: Mods.None, game: game)
        };
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);
        Assert.True(result.HasFlag(GameRejectionReason.NoValidScores));
    }

    [Fact]
    public void Process_ScoreCountCheck_LobbySizeMismatch()
    {
        Game game = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 3;
        game.Scores = new List<GameScore>
        {
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 1), team: Team.Red, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 2), team: Team.Red, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 3), team: Team.Blue, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 4), team: Team.Blue, mods: Mods.None, game: game)
        };

        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);
        Assert.True(result.HasFlag(GameRejectionReason.LobbySizeMismatch));
    }

    [Fact]
    public void Process_ScoreCountCheck_Valid()
    {
        Game game = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 2;
        game.Scores = new List<GameScore>
        {
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 1), team: Team.Red, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 2), team: Team.Red, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 3), team: Team.Blue, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 4), team: Team.Blue, mods: Mods.None, game: game)
        };

        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);
        Assert.False(result.HasFlag(GameRejectionReason.LobbySizeMismatch));
    }

    [Theory]
    [InlineData(Ruleset.Osu, Ruleset.Osu, GameRejectionReason.None)]
    [InlineData(Ruleset.Taiko, Ruleset.Osu, GameRejectionReason.RulesetMismatch)]
    public void Process_RulesetCheck(Ruleset gameRuleset, Ruleset tournamentRuleset, GameRejectionReason expected)
    {
        Game game = SeededGame.Generate(
            ruleset: gameRuleset,
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;
        game.Match.Tournament.Ruleset = tournamentRuleset;
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);
        Assert.True(result.HasFlag(expected));
    }

    [Theory]
    [InlineData(Mods.None, GameRejectionReason.None)]
    [InlineData(Mods.Hidden, GameRejectionReason.None)]
    [InlineData(Mods.SuddenDeath, GameRejectionReason.InvalidMods)]
    [InlineData(Mods.Perfect, GameRejectionReason.InvalidMods)]
    [InlineData(Mods.Relax, GameRejectionReason.InvalidMods)]
    [InlineData(Mods.Autoplay, GameRejectionReason.InvalidMods)]
    [InlineData(Mods.Relax2, GameRejectionReason.InvalidMods)]
    public void Process_ModCheck(Mods mods, GameRejectionReason expected)
    {
        Game game = SeededGame.Generate(
            mods: mods,
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);
        Assert.True(result.HasFlag(expected));
    }

    [Fact]
    public void Process_EndTimeCheck_NoEndTime()
    {
        Game game = SeededGame.Generate(
            endTime: DateTime.MinValue,
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None
        );
        game.Match.Tournament.LobbySize = 4;
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);
        Assert.True(result.HasFlag(GameRejectionReason.NoEndTime));
    }

    [Fact]
    public void Process_BeatmapUsageCheck_NoBeatmap()
    {
        Game game = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Beatmap = null;
        game.Match.Tournament.LobbySize = 2;
        game.Match.Tournament.Ruleset = Ruleset.Osu; // Ensure tournament ruleset matches game ruleset
        game.Scores = new List<GameScore>
        {
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 1), team: Team.Red, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 2), team: Team.Red, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 3), team: Team.Blue, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 4), team: Team.Blue, mods: Mods.None, game: game)
        };
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);
        Assert.Equal(GameRejectionReason.None, result);
    }

    [Fact]
    public void Process_BeatmapUsageCheck_BeatmapNotPooled()
    {
        Game game = SeededGame.Generate(
            beatmap: SeededBeatmap.Generate(osuId: 123),
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.PooledBeatmaps = new List<Beatmap> { SeededBeatmap.Generate(osuId: 456) };
        game.Match.Tournament.LobbySize = 2;
        game.Scores = new List<GameScore>
        {
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 1), team: Team.Red, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 2), team: Team.Red, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 3), team: Team.Blue, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 4), team: Team.Blue, mods: Mods.None, game: game)
        };
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);
        Assert.True(result.HasFlag(GameRejectionReason.BeatmapNotPooled));
    }

    [Fact]
    public void Process_BeatmapUsageCheck_BeatmapUsedOnce_NoPool()
    {
        Tournament tournament = SeededTournament.Generate(
            id: 1,
            name: "Test Tournament",
            abbreviation: "TT",
            ruleset: Ruleset.Osu,
            teamSize: 2
        );

        Match mainMatch = SeededMatch.Generate(id: 1, tournament: tournament);

        // Create beatmaps that will be reused
        Beatmap beatmap123 = SeededBeatmap.Generate(osuId: 123);
        Beatmap beatmap456 = SeededBeatmap.Generate(osuId: 456);

        Game game1 = SeededGame.Generate(
            id: 1,
            beatmap: beatmap123,
            match: mainMatch,
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow,
            warningFlags: GameWarningFlags.None
        );
        // Clear and re-add scores to ensure proper relationships
        game1.Scores.Clear();
        game1.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 1), team: Team.Red, mods: Mods.None, game: game1));
        game1.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 2), team: Team.Red, mods: Mods.None, game: game1));
        game1.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 3), team: Team.Blue, mods: Mods.None, game: game1));
        game1.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 4), team: Team.Blue, mods: Mods.None, game: game1));

        Game game2 = SeededGame.Generate(
            id: 2,
            beatmap: beatmap456,
            match: mainMatch,
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow,
            warningFlags: GameWarningFlags.None
        );
        game2.Scores.Clear();
        game2.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 1), team: Team.Red, mods: Mods.None, game: game2));
        game2.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 2), team: Team.Red, mods: Mods.None, game: game2));
        game2.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 3), team: Team.Blue, mods: Mods.None, game: game2));
        game2.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 4), team: Team.Blue, mods: Mods.None, game: game2));

        Game game3 = SeededGame.Generate(
            id: 3,
            beatmap: beatmap123, // Reuse the same beatmap instance as game1
            match: mainMatch,
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow,
            warningFlags: GameWarningFlags.None
        );
        game3.Scores.Clear();
        game3.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 1), team: Team.Red, mods: Mods.None, game: game3));
        game3.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 2), team: Team.Red, mods: Mods.None, game: game3));
        game3.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 3), team: Team.Blue, mods: Mods.None, game: game3));
        game3.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 4), team: Team.Blue, mods: Mods.None, game: game3));

        _checker.Process(game1, tournament);
        _checker.Process(game2, tournament);
        _checker.Process(game3, tournament);

        // beatmap123 is used twice (game1 and game3), so neither should be flagged
        // beatmap456 is used once (game2), so it should be flagged
        Assert.False(game1.WarningFlags.HasFlag(GameWarningFlags.BeatmapUsedOnce));
        Assert.True(game2.WarningFlags.HasFlag(GameWarningFlags.BeatmapUsedOnce));
        Assert.False(game3.WarningFlags.HasFlag(GameWarningFlags.BeatmapUsedOnce));
    }

    [Fact]
    public void Process_CombinedRejectionReasons_ReturnsAllFailures()
    {
        // Arrange
        Game game = SeededGame.Generate(
            teamType: TeamType.HeadToHead, // Invalid team type
            scoringType: ScoringType.Accuracy, // Invalid scoring type
            ruleset: Ruleset.Taiko, // Will mismatch with tournament
            mods: Mods.SuddenDeath, // Invalid mod
            endTime: DateTime.MinValue // No end time
        );
        game.Match.Tournament.Ruleset = Ruleset.Osu; // Different from game
        game.Match.Tournament.LobbySize = 4;
        game.Scores.Clear(); // No scores

        // Act
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);

        // Assert - Should have all rejection reasons
        Assert.True(result.HasFlag(GameRejectionReason.InvalidTeamType));
        Assert.True(result.HasFlag(GameRejectionReason.InvalidScoringType));
        Assert.True(result.HasFlag(GameRejectionReason.RulesetMismatch));
        Assert.True(result.HasFlag(GameRejectionReason.InvalidMods));
        Assert.True(result.HasFlag(GameRejectionReason.NoEndTime));
        Assert.True(result.HasFlag(GameRejectionReason.NoScores));
    }


    [Fact]
    public void Process_ScoreCountCheck_UnbalancedTeams()
    {
        // Arrange
        Game game = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 2;
        game.Scores.Clear();

        // Unbalanced teams: 3 Red, 1 Blue
        game.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 1), team: Team.Red, mods: Mods.None, game: game));
        game.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 2), team: Team.Red, mods: Mods.None, game: game));
        game.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 3), team: Team.Red, mods: Mods.None, game: game));
        game.Scores.Add(SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 4), team: Team.Blue, mods: Mods.None, game: game));

        // Act
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);

        // Assert - Should fail due to unbalanced teams
        Assert.True(result.HasFlag(GameRejectionReason.LobbySizeMismatch));
    }

    [Fact]
    public void Process_BeatmapUsageCheck_BeatmapInPool()
    {
        // Arrange
        Beatmap poolBeatmap = SeededBeatmap.Generate(osuId: 999);
        Game game = SeededGame.Generate(
            beatmap: poolBeatmap,
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.PooledBeatmaps.Add(poolBeatmap);
        game.Match.Tournament.LobbySize = 2;
        game.Scores = new List<GameScore>
        {
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 1), team: Team.Red, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 2), team: Team.Red, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 3), team: Team.Blue, mods: Mods.None, game: game),
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, player: SeededPlayer.Generate(id: 4), team: Team.Blue, mods: Mods.None, game: game)
        };

        // Act
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);

        // Assert - Should not be rejected as beatmap is in pool
        Assert.False(result.HasFlag(GameRejectionReason.BeatmapNotPooled));
    }

    [Theory]
    [InlineData(Mods.Hidden | Mods.HardRock, GameRejectionReason.None)]
    [InlineData(Mods.DoubleTime | Mods.Hidden, GameRejectionReason.None)]
    [InlineData(Mods.Nightcore | Mods.Hidden, GameRejectionReason.None)]
    [InlineData(Mods.HalfTime | Mods.Easy, GameRejectionReason.None)]
    [InlineData(Mods.Flashlight | Mods.NoFail, GameRejectionReason.None)]
    [InlineData(Mods.SuddenDeath | Mods.Hidden, GameRejectionReason.InvalidMods)]
    [InlineData(Mods.Perfect | Mods.HardRock, GameRejectionReason.InvalidMods)]
    [InlineData(Mods.Relax | Mods.DoubleTime, GameRejectionReason.InvalidMods)]
    public void Process_ModCheck_ComplexModCombinations(Mods mods, GameRejectionReason expected)
    {
        // Arrange
        Game game = SeededGame.Generate(
            mods: mods,
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;

        // Act
        GameRejectionReason result = _checker.Process(game, game.Match.Tournament);

        // Assert
        Assert.True(result.HasFlag(expected));
    }
}
