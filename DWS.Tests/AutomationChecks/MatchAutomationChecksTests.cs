using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.AutomationChecks;
using Microsoft.Extensions.Logging;
using Moq;
using TestingUtils.SeededData;
using Match = Database.Entities.Match;

namespace DWS.Tests.AutomationChecks;

public class MatchAutomationChecksTests
{
    private readonly Mock<ILogger<MatchAutomationChecks>> _loggerMock = new();
    private readonly MatchAutomationChecks _matchAutomationChecks;

    public MatchAutomationChecksTests()
    {
        _matchAutomationChecks = new MatchAutomationChecks(_loggerMock.Object);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("OWC2022: Australia VS Japan", false)]
    [InlineData("OWC2022: (Australia VS Japan)", false)]
    [InlineData("OWC2022: (Aust\n ralia)vs(Japan)", false)]
    [InlineData("OWC2022: (Australia)vs(Japan)", true)]
    [InlineData("OWC2022: (Australia) vs (Japan)", true)]
    [InlineData("OWC2022: (Australia) vs. (Japan)", true)]
    [InlineData("OWC2022: (Australia) VS (Japan)", true)]
    [InlineData("OWC2022: (Australia) VS. (Japan)", true)]
    [InlineData("조골뽑2: (양송이베이컨크림) vs (압도적줴능)", true)]
    [InlineData("FAT: ((¬‿‿¬)) vs (Pierce The Veil - Texas Is Forever [pishi's Extreme])", true)]
    public void MatchNameFormatCheck_ValidatesMatchNamesCorrectly(string matchName, bool expectedPass)
    {
        // Arrange
        MatchWarningFlags expectedWarning = expectedPass
            ? MatchWarningFlags.None
            : MatchWarningFlags.UnexpectedNameFormat;

        Tournament tournament = SeededTournament.Generate(
            abbreviation: "TEST",
            ruleset: Common.Enums.Ruleset.Osu
        );

        Match match = SeededMatch.Generate(
            name: matchName,
            warningFlags: MatchWarningFlags.None,
            tournament: tournament
        );

        // Act
        _matchAutomationChecks.Process(match, tournament);

        // Assert
        Assert.Equal(expectedWarning, match.WarningFlags);
    }

    [Theory]
    [InlineData(@"^.*((\(.+\))\s*vs\.?\s*(\(.+\))).*$", "OWC2022: (Australia) vs (Japan)", true)]
    [InlineData(@"^.*((\(.+\))\s*vs\.?\s*(\(.+\))).*$", "OWC2022: (Australia) vs. (Japan)", true)]
    [InlineData(@"^.*((\(.+\))\s*vs\.?\s*(\(.+\))).*$", "(Team A) vs (Team B)", true)]
    [InlineData(@"^.*((\(.+\))\s*vs\.?\s*(\(.+\))).*$", "Prefix: (Team A) vs (Team B) Suffix", true)]
    [InlineData(@"^.*((\(.+\))\s*vs\.?\s*(\(.+\))).*$", "Team A vs Team B", false)]
    [InlineData(@"^.*((\(.+\))\s*vs\.?\s*(\(.+\))).*$", "(Team A) vs Team B", false)]
    [InlineData(@"^.*((\(.+\))\s*vs\.?\s*(\(.+\))).*$", "Team A vs (Team B)", false)]
    public void RegexPattern_MatchesExpectedFormats(string pattern, string input, bool expectedMatch)
    {
        // This test validates that the regex pattern correctly matches expected formats
        bool match = System.Text.RegularExpressions.Regex.IsMatch(input, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        Assert.Equal(expectedMatch, match);
    }

    [Fact]
    public void Process_MatchEndTimeCheck_NoEndTime_ReturnsRejection()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(abbreviation: "TEST", ruleset: Common.Enums.Ruleset.Osu);
        Match match = SeededMatch.Generate(name: "TEST: (A) vs (B)", tournament: tournament);
        match.EndTime = null;

        // Act
        MatchRejectionReason result = _matchAutomationChecks.Process(match, tournament);

        // Assert
        Assert.True(result.HasFlag(MatchRejectionReason.NoEndTime));
    }

    [Fact]
    public void Process_MatchEndTimeCheck_HasEndTime_ReturnsNone()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(abbreviation: "TEST", ruleset: Common.Enums.Ruleset.Osu);
        Match match = SeededMatch.Generate(name: "TEST: (A) vs (B)", tournament: tournament);
        match.EndTime = DateTime.UtcNow;

        // Act
        MatchRejectionReason result = _matchAutomationChecks.Process(match, tournament);

        // Assert
        Assert.False(result.HasFlag(MatchRejectionReason.NoEndTime));
    }

    [Fact]
    public void Process_MatchGameCountCheck_NoGames_ReturnsNoGames()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(abbreviation: "TEST", ruleset: Common.Enums.Ruleset.Osu);
        Match match = SeededMatch.Generate(name: "TEST: (A) vs (B)", tournament: tournament);
        match.Games.Clear();
        match.EndTime = DateTime.UtcNow;

        // Act
        MatchRejectionReason result = _matchAutomationChecks.Process(match, tournament);

        // Assert
        Assert.True(result.HasFlag(MatchRejectionReason.NoGames));
    }

    [Fact]
    public void Process_MatchGameCountCheck_NoValidGames_ReturnsNoValidGames()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(abbreviation: "TEST", ruleset: Common.Enums.Ruleset.Osu);
        Match match = SeededMatch.Generate(name: "TEST: (A) vs (B)", tournament: tournament);
        match.EndTime = DateTime.UtcNow;

        // Add only rejected games
        Game game1 = SeededGame.Generate(match: match, verificationStatus: VerificationStatus.Rejected);
        Game game2 = SeededGame.Generate(match: match, verificationStatus: VerificationStatus.PreRejected);
        match.Games.Add(game1);
        match.Games.Add(game2);

        // Act
        MatchRejectionReason result = _matchAutomationChecks.Process(match, tournament);

        // Assert
        Assert.True(result.HasFlag(MatchRejectionReason.NoValidGames));
    }


    [Theory]
    [InlineData("TEST", "TEST: (A) vs (B)", MatchRejectionReason.None)]
    [InlineData("OWC", "OWC2022: (A) vs (B)", MatchRejectionReason.None)]
    [InlineData("TEST", "WRONG: (A) vs (B)", MatchRejectionReason.NamePrefixMismatch)]
    [InlineData("ABC", "XYZ: (A) vs (B)", MatchRejectionReason.NamePrefixMismatch)]
    public void Process_MatchNamePrefixCheck(string abbreviation, string matchName, MatchRejectionReason expected)
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(abbreviation: abbreviation, ruleset: Common.Enums.Ruleset.Osu);
        Match match = SeededMatch.Generate(name: matchName, tournament: tournament);
        match.EndTime = DateTime.UtcNow;

        // Act
        MatchRejectionReason result = _matchAutomationChecks.Process(match, tournament);

        // Assert
        if (expected != MatchRejectionReason.None)
        {
            Assert.True(result.HasFlag(expected));
        }
        else
        {
            Assert.False(result.HasFlag(MatchRejectionReason.NamePrefixMismatch));
        }
    }


    [Fact]
    public void Process_HeadToHeadConversion_NotAppliedForNon1v1Tournaments()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(abbreviation: "TEST", ruleset: Common.Enums.Ruleset.Osu, teamSize: 4);
        Match match = SeededMatch.Generate(name: "TEST: (A) vs (B)", tournament: tournament);
        match.EndTime = DateTime.UtcNow;
        match.Games.Clear();

        // Create HeadToHead games
        Game game = SeededGame.Generate(match: match, teamType: TeamType.HeadToHead, verificationStatus: VerificationStatus.PreVerified);
        match.Games.Add(game);

        // Act
        _matchAutomationChecks.Process(match, tournament);

        // Assert - Should remain HeadToHead
        Assert.Equal(TeamType.HeadToHead, game.TeamType);
    }

    [Fact]
    public void Process_CombinedChecks_MultipleRejectionReasons()
    {
        // Arrange
        Tournament tournament = SeededTournament.Generate(abbreviation: "TEST", ruleset: Common.Enums.Ruleset.Osu);
        Match match = SeededMatch.Generate(name: "WRONG: Invalid Format", tournament: tournament);
        match.EndTime = null; // No end time
        match.Games.Clear(); // No games

        // Act
        MatchRejectionReason result = _matchAutomationChecks.Process(match, tournament);

        // Assert - Should have multiple rejection reasons and warning flags
        Assert.True(result.HasFlag(MatchRejectionReason.NoEndTime));
        Assert.True(result.HasFlag(MatchRejectionReason.NoGames));
        Assert.True(result.HasFlag(MatchRejectionReason.NamePrefixMismatch));
        Assert.True(match.WarningFlags.HasFlag(MatchWarningFlags.UnexpectedNameFormat));
    }

}
