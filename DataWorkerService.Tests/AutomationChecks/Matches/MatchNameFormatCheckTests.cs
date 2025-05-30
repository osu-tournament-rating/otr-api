using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.AutomationChecks.Matches;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Matches;

public class MatchNameFormatCheckTests : AutomationChecksTestBase<MatchNameFormatCheck>
{
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
    [InlineData("FAT: ((\u2500‿‿\u2500)) vs (Pierce The Veil - Texas Is Forever [pishi’s Extreme])", true)]
    public void Check_PassesWhenExpected(string matchName, bool expectedPass)
    {
        // Arrange
        MatchWarningFlags expectedWarning = expectedPass
            ? MatchWarningFlags.None
            : MatchWarningFlags.UnexpectedNameFormat;

        Match match = SeededMatch.Generate(name: matchName, warningFlags: MatchWarningFlags.None);

        // Act
        bool actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedWarning, match.WarningFlags);
    }
}
