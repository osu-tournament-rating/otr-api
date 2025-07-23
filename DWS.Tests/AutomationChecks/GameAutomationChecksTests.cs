
using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.AutomationChecks;
using Microsoft.Extensions.Logging;
using Moq;
using TestingUtils.SeededData;

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
        var game = SeededGame.Generate(
            teamType: teamType,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(expected));
    }

    [Theory]
    [InlineData(ScoringType.ScoreV2, GameRejectionReason.None)]
    [InlineData(ScoringType.Score, GameRejectionReason.InvalidScoringType)]
    [InlineData(ScoringType.Accuracy, GameRejectionReason.InvalidScoringType)]
    [InlineData(ScoringType.Combo, GameRejectionReason.InvalidScoringType)]
    public void Process_ScoringTypeCheck(ScoringType scoringType, GameRejectionReason expected)
    {
        var game = SeededGame.Generate(
            scoringType: scoringType,
            teamType: TeamType.TeamVs,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(expected));
    }

    [Fact]
    public void Process_ScoreCountCheck_NoScores()
    {
        var game = SeededGame.Generate(
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;
        game.Scores = new List<GameScore>();
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(GameRejectionReason.NoScores));
    }

    [Fact]
    public void Process_ScoreCountCheck_NoValidScores()
    {
        var game = SeededGame.Generate(
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
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(GameRejectionReason.NoValidScores));
    }

    [Fact]
    public void Process_ScoreCountCheck_LobbySizeMismatch()
    {
        var game = SeededGame.Generate(
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

        var result = _checker.Process(game);
        Assert.True(result.HasFlag(GameRejectionReason.LobbySizeMismatch));
    }

    [Fact]
    public void Process_ScoreCountCheck_Valid()
    {
        var game = SeededGame.Generate(
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

        var result = _checker.Process(game);
        Assert.False(result.HasFlag(GameRejectionReason.LobbySizeMismatch));
    }

    [Theory]
    [InlineData(Ruleset.Osu, Ruleset.Osu, GameRejectionReason.None)]
    [InlineData(Ruleset.Taiko, Ruleset.Osu, GameRejectionReason.RulesetMismatch)]
    public void Process_RulesetCheck(Ruleset gameRuleset, Ruleset tournamentRuleset, GameRejectionReason expected)
    {
        var game = SeededGame.Generate(
            ruleset: gameRuleset,
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            mods: Mods.None,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;
        game.Match.Tournament.Ruleset = tournamentRuleset;
        var result = _checker.Process(game);
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
        var game = SeededGame.Generate(
            mods: mods,
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            endTime: DateTime.UtcNow
        );
        game.Match.Tournament.LobbySize = 4;
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(expected));
    }

    [Fact]
    public void Process_EndTimeCheck_NoEndTime()
    {
        var game = SeededGame.Generate(
            endTime: DateTime.MinValue,
            teamType: TeamType.TeamVs,
            scoringType: ScoringType.ScoreV2,
            ruleset: Ruleset.Osu,
            mods: Mods.None
        );
        game.Match.Tournament.LobbySize = 4;
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(GameRejectionReason.NoEndTime));
    }

    [Fact]
    public void Process_BeatmapUsageCheck_NoBeatmap()
    {
        var game = SeededGame.Generate(
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
        var result = _checker.Process(game);
        Assert.Equal(GameRejectionReason.None, result);
    }

    [Fact]
    public void Process_BeatmapUsageCheck_BeatmapNotPooled()
    {
        var game = SeededGame.Generate(
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
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(GameRejectionReason.BeatmapNotPooled));
    }

    [Fact]
    public void Process_BeatmapUsageCheck_BeatmapUsedOnce_NoPool()
    {
        var tournament = SeededTournament.Generate(
            id: 1,
            name: "Test Tournament",
            abbreviation: "TT",
            ruleset: Ruleset.Osu,
            teamSize: 2
        );

        var mainMatch = SeededMatch.Generate(id: 1, tournament: tournament);

        // Create beatmaps that will be reused
        var beatmap123 = SeededBeatmap.Generate(osuId: 123);
        var beatmap456 = SeededBeatmap.Generate(osuId: 456);

        var game1 = SeededGame.Generate(
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

        var game2 = SeededGame.Generate(
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

        var game3 = SeededGame.Generate(
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

        _checker.Process(game1);
        _checker.Process(game2);
        _checker.Process(game3);

        // beatmap123 is used twice (game1 and game3), so neither should be flagged
        // beatmap456 is used once (game2), so it should be flagged
        Assert.False(game1.WarningFlags.HasFlag(GameWarningFlags.BeatmapUsedOnce));
        Assert.True(game2.WarningFlags.HasFlag(GameWarningFlags.BeatmapUsedOnce));
        Assert.False(game3.WarningFlags.HasFlag(GameWarningFlags.BeatmapUsedOnce));
    }
}
