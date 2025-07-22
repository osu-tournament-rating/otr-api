
using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.AutomationChecks;
using Microsoft.Extensions.Logging;
using Moq;
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

    private static Game CreateGame(Action<Game> configure, Tournament tournament = null)
    {
        tournament ??= new Tournament
            {
                Id = 1,
                Name = "Test Tournament",
                Abbreviation = "TT",
                Ruleset = Ruleset.Osu,
                LobbySize = 4,
                Matches = new List<Match>()
            };

        var match = new Match
        {
            Id = 1,
            OsuId = 12345,
            Name = "TT: (Team A) vs (Team B)",
            Tournament = tournament
        };

        tournament.Matches.Add(match);

        var game = new Game
        {
            Id = 1,
            OsuId = 54321,
            Match = match,
            ScoringType = ScoringType.ScoreV2,
            TeamType = TeamType.TeamVs,
            Ruleset = Ruleset.Osu,
            Mods = Mods.None,
            EndTime = DateTime.UtcNow,
            Scores = new List<GameScore>()
        };

        configure(game);
        return game;
    }

    [Theory]
    [InlineData(TeamType.TeamVs, GameRejectionReason.None)]
    [InlineData(TeamType.HeadToHead, GameRejectionReason.InvalidTeamType)]
    [InlineData(TeamType.TagCoop, GameRejectionReason.InvalidTeamType)]
    [InlineData(TeamType.TagTeamVs, GameRejectionReason.InvalidTeamType)]
    public void Process_TeamTypeCheck(TeamType teamType, GameRejectionReason expected)
    {
        var game = CreateGame(g => g.TeamType = teamType);
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
        var game = CreateGame(g => g.ScoringType = scoringType);
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(expected));
    }

    [Fact]
    public void Process_ScoreCountCheck_NoScores()
    {
        var game = CreateGame(g => g.Scores = new List<GameScore>());
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(GameRejectionReason.NoScores));
    }

    [Fact]
    public void Process_ScoreCountCheck_NoValidScores()
    {
        var game = CreateGame(g =>
        {
            g.Scores = new List<GameScore>
            {
                new() { VerificationStatus = VerificationStatus.Rejected },
                new() { VerificationStatus = VerificationStatus.PreRejected }
            };
        });
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(GameRejectionReason.NoValidScores));
    }

    [Fact]
    public void Process_ScoreCountCheck_LobbySizeMismatch()
    {
        var game = CreateGame(g =>
        {
            g.Scores = new List<GameScore>
            {
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 1 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 2 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 3 }, Team = Team.Blue },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 4 }, Team = Team.Blue }
            };
            g.Match.Tournament.LobbySize = 3; // Expects 6 players, gets 4
        });

        var result = _checker.Process(game);
        Assert.True(result.HasFlag(GameRejectionReason.LobbySizeMismatch));
    }

    [Fact]
    public void Process_ScoreCountCheck_Valid()
    {
        var game = CreateGame(g =>
        {
            g.Match.Tournament.LobbySize = 2;
            g.Scores = new List<GameScore>
            {
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 1 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 2 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 3 }, Team = Team.Blue },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 4 }, Team = Team.Blue }
            };
        });

        var result = _checker.Process(game);
        Assert.False(result.HasFlag(GameRejectionReason.LobbySizeMismatch));
    }

    [Theory]
    [InlineData(Ruleset.Osu, Ruleset.Osu, GameRejectionReason.None)]
    [InlineData(Ruleset.Taiko, Ruleset.Osu, GameRejectionReason.RulesetMismatch)]
    public void Process_RulesetCheck(Ruleset gameRuleset, Ruleset tournamentRuleset, GameRejectionReason expected)
    {
        var game = CreateGame(g =>
        {
            g.Ruleset = gameRuleset;
            g.Match.Tournament.Ruleset = tournamentRuleset;
        });
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
        var game = CreateGame(g => g.Mods = mods);
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(expected));
    }

    [Fact]
    public void Process_EndTimeCheck_NoEndTime()
    {
        var game = CreateGame(g => g.EndTime = DateTime.MinValue);
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(GameRejectionReason.NoEndTime));
    }

    [Fact]
    public void Process_BeatmapUsageCheck_NoBeatmap()
    {
        var game = CreateGame(g =>
        {
            g.Beatmap = null;
            g.Match.Tournament.LobbySize = 2;
            g.Scores = new List<GameScore>
            {
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 1 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 2 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 3 }, Team = Team.Blue },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 4 }, Team = Team.Blue }
            };
        });
        var result = _checker.Process(game);
        Assert.Equal(GameRejectionReason.None, result);
    }

    [Fact]
    public void Process_BeatmapUsageCheck_BeatmapNotPooled()
    {
        var game = CreateGame(g =>
        {
            g.Beatmap = new Beatmap { OsuId = 123 };
            g.Match.Tournament.PooledBeatmaps = new List<Beatmap> { new() { OsuId = 456 } };
            g.Match.Tournament.LobbySize = 2;
            g.Scores = new List<GameScore>
            {
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 1 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 2 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 3 }, Team = Team.Blue },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 4 }, Team = Team.Blue }
            };
        });
        var result = _checker.Process(game);
        Assert.True(result.HasFlag(GameRejectionReason.BeatmapNotPooled));
    }

    [Fact]
    public void Process_BeatmapUsageCheck_BeatmapUsedOnce_NoPool()
    {
        var tournament = new Tournament
        {
            Id = 1,
            Name = "Test Tournament",
            Abbreviation = "TT",
            Ruleset = Ruleset.Osu,
            LobbySize = 2,
            Matches = new List<Match>()
        };

        var game1 = CreateGame(g =>
        {
            g.Beatmap = new Beatmap { OsuId = 123 };
            g.Scores = new List<GameScore>
            {
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 1 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 2 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 3 }, Team = Team.Blue },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 4 }, Team = Team.Blue }
            };
        }, tournament);

        var game2 = CreateGame(g =>
        {
            g.Beatmap = new Beatmap { OsuId = 456 };
            g.Scores = new List<GameScore>
            {
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 1 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 2 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 3 }, Team = Team.Blue },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 4 }, Team = Team.Blue }
            };
        }, tournament);

        var game3 = CreateGame(g =>
        {
            g.Beatmap = new Beatmap { OsuId = 123 };
            g.Scores = new List<GameScore>
            {
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 1 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 2 }, Team = Team.Red },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 3 }, Team = Team.Blue },
                new() { VerificationStatus = VerificationStatus.Verified, Player = new Player { Id = 4 }, Team = Team.Blue }
            };
        }, tournament);

        tournament.Matches.Clear();
        tournament.Matches.Add(new Match { Id = 1, Games = new List<Game> { game1, game2, game3 } });

        _checker.Process(game1);
        _checker.Process(game2);
        _checker.Process(game3);

        Assert.False(game1.WarningFlags.HasFlag(GameWarningFlags.BeatmapUsedOnce));
        Assert.True(game2.WarningFlags.HasFlag(GameWarningFlags.BeatmapUsedOnce));
        Assert.False(game3.WarningFlags.HasFlag(GameWarningFlags.BeatmapUsedOnce));
    }
}
