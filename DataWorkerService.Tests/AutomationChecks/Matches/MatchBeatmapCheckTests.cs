using Common.Enums.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks.Matches;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Matches;

public class MatchBeatmapCheckTests : AutomationChecksTestBase<MatchBeatmapCheck>
{
    [Fact]
    public void Check_Flagged_WhenGamesContainUnexpectedBeatmaps()
    {
        // Arrange
        const MatchWarningFlags expectedWarningFlags = MatchWarningFlags.UnexpectedBeatmapsFound;
        Match match = SeededMatch.Generate(warningFlags: MatchWarningFlags.None);

        for (var i = 0; i < 5; i++)
        {
            DateTime time = SeededDate.Placeholder.AddSeconds(i);
            Game game = i > 1
                ? SeededGame.Generate(rejectionReason: GameRejectionReason.BeatmapNotPooled, startTime: time)
                : SeededGame.Generate(rejectionReason: GameRejectionReason.None, startTime: time);

            match.Games.Add(game);
            game.Match = match;
        }

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.False(actualPass);
        Assert.Equal(expectedWarningFlags, match.WarningFlags);
    }

    [Fact]
    public void Check_NotFlagged_WhenFirstTwoGames_ContainUnexpectedBeatmaps()
    {
        // Arrange
        const MatchWarningFlags expectedWarningFlags = MatchWarningFlags.None;
        Match match = SeededMatch.Generate(warningFlags: MatchWarningFlags.None);

        for (var i = 0; i < 5; i++)
        {
            DateTime time = SeededDate.Placeholder.AddSeconds(i);
            Game game = i < 2
                ? SeededGame.Generate(rejectionReason: GameRejectionReason.BeatmapNotPooled, startTime: time)
                : SeededGame.Generate(rejectionReason: GameRejectionReason.None, startTime: time);

            match.Games.Add(game);
            game.Match = match;
        }

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(expectedWarningFlags, match.WarningFlags);
    }

    [Fact]
    public void Check_NotFlagged_WhenGamesDoNotContainUnexpectedBeatmaps()
    {
        // Arrange
        const MatchWarningFlags expectedWarningFlags = MatchWarningFlags.None;
        Match match = SeededMatch.Generate(warningFlags: MatchWarningFlags.None);

        for (var i = 0; i < 5; i++)
        {
            DateTime time = SeededDate.Placeholder.AddSeconds(i);
            Game game = SeededGame.Generate(rejectionReason: GameRejectionReason.None, startTime: time);

            match.Games.Add(game);
            game.Match = match;
        }

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.True(actualPass);
        Assert.Equal(expectedWarningFlags, match.WarningFlags);
    }
}
