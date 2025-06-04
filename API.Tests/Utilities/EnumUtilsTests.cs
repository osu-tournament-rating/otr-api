using Common.Enums.Verification;
using Common.Utilities.Extensions;

namespace APITests.Utilities;

public class VerificationStatusExtensionsTests
{
    [Theory]
    [InlineData(VerificationStatus.PreRejected, VerificationStatus.Rejected)]
    [InlineData(VerificationStatus.PreVerified, VerificationStatus.Verified)]
    [InlineData(VerificationStatus.Rejected, VerificationStatus.Rejected)]
    [InlineData(VerificationStatus.Verified, VerificationStatus.Verified)]
    [InlineData(VerificationStatus.None, VerificationStatus.None)]
    public void VerificationStatus_ConfirmPreStatus_BehavesCorrectly(VerificationStatus current, VerificationStatus expected)
    {
        // Act
        VerificationStatus actual = current.ConfirmPreStatus();

        // Assert
        Assert.Equal(expected, actual);
    }
}
