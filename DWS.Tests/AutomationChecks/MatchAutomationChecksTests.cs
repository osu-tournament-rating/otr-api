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
}
