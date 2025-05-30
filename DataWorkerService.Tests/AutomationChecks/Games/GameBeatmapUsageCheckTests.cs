using Common.Enums.Verification;
using Database.Entities;
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
        bool actualPass = AutomationCheck.Check(game);

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
        game.Beatmap = new Beatmap { Id = beatmapOsuId, OsuId = beatmapOsuId };

        // Act
        bool actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.True(actualPass);
        Assert.False(game.RejectionReason.HasFlag(expectedRejectionReason));
    }

    [Fact]
    public void Check_GameFlag_IsBeatmapUsedOnce_WhenBeatmapUsedOnce()
    {
        // Arrange
        const int beatmapOsuId = 500;
        const GameWarningFlags expectedFlag = GameWarningFlags.BeatmapUsedOnce;

        Tournament tournament = GenerateTournamentWithPooledBeatmaps();
        tournament.PooledBeatmaps = [];

        Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None,
            warningFlags: MatchWarningFlags.None, tournament: tournament);
        Beatmap beatmap = SeededBeatmap.Generate(-1, beatmapOsuId);
        Game _ = SeededGame.Generate(rejectionReason: GameRejectionReason.None, warningFlags: GameWarningFlags.None,
            match: match, beatmap: beatmap);

        Game relevantGame = tournament.Matches.SelectMany(m => m.Games).First(g => g.Beatmap!.OsuId == beatmapOsuId);

        // Act
        bool actualPass = AutomationCheck.Check(relevantGame);
        bool unique = tournament.Matches
            .SelectMany(m => m.Games)
            .Select(g => g.Beatmap)
            .Select(b => b!.OsuId)
            .Count(osuId => osuId == beatmapOsuId) == 1;

        // Assert
        Assert.Empty(tournament.PooledBeatmaps);
        Assert.True(actualPass);
        Assert.True(unique);
        Assert.True(relevantGame.WarningFlags.HasFlag(expectedFlag));
    }

    /// <summary>
    /// Generates a tournament with matches and games which have beatmaps.
    /// There are 5 matches each with 1 game. Each game's beatmap is pooled
    /// in the tournament.
    /// </summary>
    private static Tournament GenerateTournamentWithPooledBeatmaps()
    {
        Tournament tournament = SeededTournament.Generate(rejectionReason: TournamentRejectionReason.None);

        for (int i = 0; i < 5; i++)
        {
            Match match = SeededMatch.Generate(rejectionReason: MatchRejectionReason.None, tournament: tournament);

            Game game = SeededGame.Generate(rejectionReason: GameRejectionReason.None,
                warningFlags: GameWarningFlags.None, match: match);

            Beatmap beatmap = SeededBeatmap.Generate(id: i, osuId: i);
            game.Beatmap = beatmap;

            match.Games.Add(game);
            tournament.Matches.Add(match);
            tournament.PooledBeatmaps.Add(beatmap);
        }

        return tournament;
    }
}
