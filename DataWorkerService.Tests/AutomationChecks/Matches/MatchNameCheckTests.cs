using Database.Entities;
using Database.Enums.Verification;
using DataWorkerService.AutomationChecks.Matches;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Matches;

public class MatchNameCheckTests : AutomationChecksTestBase<MatchNameCheck>
{
    [Theory]
    [InlineData("(Australia) VS (Japan)", "OWC2022", false)]
    [InlineData("OWC: (Australia) VS (Japan)", "OWC2022", false)]
    [InlineData("OWC2022: (Australia)vs(Japan)", "OWC2022", true)]
    [InlineData("OWC2022: (Australia) vs (Japan)", "OWC2022", true)]
    [InlineData("조골뽑2: (양송이베이컨크림) vs (압도적줴능)", "조골뽑2", true)]
    [InlineData("FAT: ((\u2500‿‿\u2500)) vs (Pierce The Veil - Texas Is Forever [pishi’s Extreme])", "FAT", true)]
    public void Check_PassesWhenExpected(string matchName, string tournamentAbbreviation, bool expectedPass)
    {
        // Arrange
        MatchRejectionReason expectedRejectionReason = expectedPass
            ? MatchRejectionReason.None
            : MatchRejectionReason.InvalidName;

        Match match = SeededMatch.Generate(name: matchName, rejectionReason: MatchRejectionReason.None);
        match.Tournament.Abbreviation = tournamentAbbreviation;

        // Act
        var actualPass = AutomationCheck.Check(match);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedRejectionReason, match.RejectionReason);
    }
}
