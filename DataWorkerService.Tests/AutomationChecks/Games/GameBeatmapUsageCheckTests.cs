using Database.Entities;
using Database.Enums.Verification;
using DataWorkerService.AutomationChecks.Games;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Games;

public class GameBeatmapUsageCheckTests : AutomationChecksTestBase<GameBeatmapUsageCheck>
{
    [Fact]
    public void Check_GameRejected_WhenBeatmapNotPooled()
    {
        // Arrange
        const int beatmapOsuId = 100;
        const GameRejectionReason expectedRejectionReason = GameRejectionReason.BeatmapNotPooled;

        Tournament tournament = GenerateTournamentWithPooledBeatmaps();
        Game game = tournament.Matches.First().Games.First();
        game.Beatmap = new Beatmap { OsuId = beatmapOsuId };

        // Act
        var actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.False(actualPass);
        Assert.True(game.RejectionReason.HasFlag(expectedRejectionReason));
    }

    [Fact]
    public void Check_GameNotRejected_WhenBeatmapPooled()
    {
        // Arrange
        const int beatmapOsuId = 1;
        const GameRejectionReason expectedRejectionReason = GameRejectionReason.BeatmapNotPooled;

        Tournament tournament = GenerateTournamentWithPooledBeatmaps();
        Game game = tournament.Matches.First().Games.First();
        game.Beatmap = new Beatmap { OsuId = beatmapOsuId };

        // Act
        var actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.True(actualPass);
        Assert.False(game.RejectionReason.HasFlag(expectedRejectionReason));
    }

    [Fact]
    public void Check_GameFlag_IsBeatmapUsedOnce_WhenBeatmapUsedOnce()
    {
        // Arrange
        const int beatmapOsuId = 1;
        const GameWarningFlags expectedFlag = GameWarningFlags.BeatmapUsedOnce;

        Tournament tournament = GenerateTournamentWithPooledBeatmaps();
        tournament.PooledBeatmaps = new List<Beatmap>();

        Game game = tournament.Matches.SelectMany(m => m.Games).First(g => g.Beatmap!.OsuId == beatmapOsuId);

        // Act
        var actualPass = AutomationCheck.Check(game);
        var unique = tournament.Matches
            .SelectMany(m => m.Games)
            .Select(g => g.Beatmap)
            .Select(b => b!.OsuId)
            .Count(id => id == beatmapOsuId) == 1;

        // Assert
        Assert.Empty(tournament.PooledBeatmaps);
        Assert.True(actualPass);
        Assert.True(unique);
        Assert.True(game.WarningFlags.HasFlag(expectedFlag));
    }

    /// <summary>
    /// Generates a tournament with matches and games which have beatmaps.
    /// There are 5 matches each with 1 game. Each game's beatmap is pooled
    /// in the tournament.
    /// </summary>
    private static Tournament GenerateTournamentWithPooledBeatmaps()
    {
        Tournament tournament = SeededTournament.Generate();

        for (var i = 0; i < 5; i++)
        {
            Match match = SeededMatch.Generate();
            match.Tournament = tournament;

            Game game = SeededGame.Generate(warningFlags: GameWarningFlags.None);
            game.Match = match;

            Beatmap beatmap = SeededBeatmap.Generate(id: i, osuId: i);

            game.Beatmap = beatmap;
            match.Games.Add(game);
            tournament.Matches.Add(match);

            tournament.PooledBeatmaps.Add(beatmap);
        }

        return tournament;
    }
}
