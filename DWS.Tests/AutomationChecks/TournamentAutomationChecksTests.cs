using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.AutomationChecks;
using Microsoft.Extensions.Logging;
using Moq;
using TestingUtils.SeededData;
using Xunit;

namespace DWS.Tests.AutomationChecks;

public class TournamentAutomationChecksTests
{
    private readonly Mock<ILogger<TournamentAutomationChecks>> _loggerMock = new();
    private readonly TournamentAutomationChecks _tournamentAutomationChecks;

    public TournamentAutomationChecksTests()
    {
        _tournamentAutomationChecks = new TournamentAutomationChecks(_loggerMock.Object);
    }

    private static Tournament CreateTestTournament()
    {
        var tournament = SeededTournament.Generate(
            id: 1,
            name: "Test Tournament",
            abbreviation: "TT",
            forumUrl: "https://osu.ppy.sh/community/forums/topics/123456",
            rankRangeLowerBound: 1000,
            ruleset: Ruleset.Osu,
            teamSize: 1
        );

        // Need to create a new tournament instance with SubmittedByUser set
        // since it's an init-only property
        return new Tournament
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Abbreviation = tournament.Abbreviation,
            ForumUrl = tournament.ForumUrl,
            RankRangeLowerBound = tournament.RankRangeLowerBound,
            Ruleset = tournament.Ruleset,
            LobbySize = tournament.LobbySize,
            VerificationStatus = tournament.VerificationStatus,
            RejectionReason = tournament.RejectionReason,
            ProcessingStatus = tournament.ProcessingStatus,
            SubmittedByUser = new User
            {
                Id = 1,
                Player = SeededPlayer.Generate(id: 1, username: "TestUser", osuId: 12345)
            },
            Matches = new List<Database.Entities.Match>()
        };
    }

    [Fact]
    public void Process_NoVerifiedMatches_ReturnsNoVerifiedMatches()
    {
        // Arrange
        var tournament = CreateTestTournament();

        // Add matches that are all not verified
        var match1 = SeededMatch.Generate(tournament: tournament);
        match1.VerificationStatus = VerificationStatus.PreRejected;
        tournament.Matches.Add(match1);

        var match2 = SeededMatch.Generate(tournament: tournament);
        match2.VerificationStatus = VerificationStatus.PreRejected;
        tournament.Matches.Add(match2);

        // Act
        var result = _tournamentAutomationChecks.Process(tournament);

        // Assert
        Assert.Equal(TournamentRejectionReason.NoVerifiedMatches, result);
    }

    [Fact]
    public void Process_AllMatchesVerified_ReturnsNone()
    {
        // Arrange
        var tournament = CreateTestTournament();

        // Add matches that are all verified
        var match1 = SeededMatch.Generate(tournament: tournament);
        match1.VerificationStatus = VerificationStatus.PreVerified;
        var game1 = SeededGame.Generate(match: match1);
        match1.Games.Add(game1);
        tournament.Matches.Add(match1);

        var match2 = SeededMatch.Generate(tournament: tournament);
        match2.VerificationStatus = VerificationStatus.PreVerified;
        var game2 = SeededGame.Generate(match: match2);
        match2.Games.Add(game2);
        tournament.Matches.Add(match2);

        // Act
        var result = _tournamentAutomationChecks.Process(tournament);

        // Assert
        Assert.Equal(TournamentRejectionReason.None, result);
    }

    [Theory]
    [InlineData(0, 10, TournamentRejectionReason.NoVerifiedMatches)] // 0% verified
    [InlineData(5, 10, TournamentRejectionReason.NotEnoughVerifiedMatches)] // 50% verified
    [InlineData(7, 10, TournamentRejectionReason.NotEnoughVerifiedMatches)] // 70% verified
    [InlineData(8, 10, TournamentRejectionReason.None)] // 80% verified (threshold)
    [InlineData(9, 10, TournamentRejectionReason.None)] // 90% verified
    [InlineData(10, 10, TournamentRejectionReason.None)] // 100% verified
    public void Process_VariousVerificationPercentages_ReturnsExpectedReason(
        int verifiedCount,
        int totalCount,
        TournamentRejectionReason expectedReason)
    {
        // Arrange
        var tournament = CreateTestTournament();

        // Add verified matches
        for (int i = 0; i < verifiedCount; i++)
        {
            var match = SeededMatch.Generate(tournament: tournament);
            match.VerificationStatus = VerificationStatus.PreVerified;
            var game = SeededGame.Generate(match: match);
            match.Games.Add(game);
            tournament.Matches.Add(match);
        }

        // Add unverified matches
        for (int i = verifiedCount; i < totalCount; i++)
        {
            var match = SeededMatch.Generate(tournament: tournament);
            match.VerificationStatus = VerificationStatus.PreRejected;
            var game = SeededGame.Generate(match: match);
            match.Games.Add(game);
            tournament.Matches.Add(match);
        }

        // Act
        var result = _tournamentAutomationChecks.Process(tournament);

        // Assert
        Assert.Equal(expectedReason, result);
    }

    [Fact]
    public void Process_MatchesWithoutGamesExcludedFromCount_ReturnsCorrectReason()
    {
        // Arrange
        var tournament = CreateTestTournament();

        // Add 8 matches with games (6 verified, 2 not verified = 75% < 80%)
        for (int i = 0; i < 6; i++)
        {
            var match = SeededMatch.Generate(tournament: tournament);
            match.VerificationStatus = VerificationStatus.PreVerified;
            var game = SeededGame.Generate(match: match);
            match.Games.Add(game);
            tournament.Matches.Add(match);
        }

        for (int i = 0; i < 2; i++)
        {
            var match = SeededMatch.Generate(tournament: tournament);
            match.VerificationStatus = VerificationStatus.PreRejected;
            var game = SeededGame.Generate(match: match);
            match.Games.Add(game);
            tournament.Matches.Add(match);
        }

        // Add 5 matches without games (should be excluded from percentage calculation)
        for (int i = 0; i < 5; i++)
        {
            var match = SeededMatch.Generate(tournament: tournament);
            match.VerificationStatus = VerificationStatus.PreRejected;
            // No games added
            tournament.Matches.Add(match);
        }

        // Act
        var result = _tournamentAutomationChecks.Process(tournament);

        // Assert
        // 6 out of 8 matches with games are verified (75%), which is less than 80%
        Assert.Equal(TournamentRejectionReason.NotEnoughVerifiedMatches, result);
    }

    [Fact]
    public void Process_MatchesWithoutGamesButAllOthersVerified_ReturnsNone()
    {
        // Arrange
        var tournament = CreateTestTournament();

        // Add 10 matches with games (all verified)
        for (int i = 0; i < 10; i++)
        {
            var match = SeededMatch.Generate(tournament: tournament);
            match.VerificationStatus = VerificationStatus.PreVerified;
            var game = SeededGame.Generate(match: match);
            match.Games.Add(game);
            tournament.Matches.Add(match);
        }

        // Add 3 matches without games
        for (int i = 0; i < 3; i++)
        {
            var match = SeededMatch.Generate(tournament: tournament);
            match.VerificationStatus = VerificationStatus.PreRejected;
            // No games added
            tournament.Matches.Add(match);
        }

        // Act
        var result = _tournamentAutomationChecks.Process(tournament);

        // Assert
        // All 10 matches with games are verified (100%)
        Assert.Equal(TournamentRejectionReason.None, result);
    }

    [Fact]
    public void Process_EmptyTournament_ReturnsNoVerifiedMatches()
    {
        // Arrange
        var tournament = CreateTestTournament();
        // No matches added

        // Act
        var result = _tournamentAutomationChecks.Process(tournament);

        // Assert
        Assert.Equal(TournamentRejectionReason.NoVerifiedMatches, result);
    }

    [Fact]
    public void Process_OnlyMatchesWithoutGames_ReturnsNoVerifiedMatches()
    {
        // Arrange
        var tournament = CreateTestTournament();

        // Add only matches without games
        for (int i = 0; i < 5; i++)
        {
            var match = SeededMatch.Generate(tournament: tournament);
            // No games added
            tournament.Matches.Add(match);
        }

        // Act
        var result = _tournamentAutomationChecks.Process(tournament);

        // Assert
        Assert.Equal(TournamentRejectionReason.NoVerifiedMatches, result);
    }

}
