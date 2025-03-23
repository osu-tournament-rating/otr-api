using Common.Enums.Verification;
using Database.Utilities;

namespace APITests.Utilities;

public class EnumUtilsTests
{
    [Theory]
    [InlineData(VerificationStatus.PreRejected, VerificationStatus.Rejected)]
    [InlineData(VerificationStatus.PreVerified, VerificationStatus.Verified)]
    [InlineData(VerificationStatus.Rejected, VerificationStatus.Rejected)]
    [InlineData(VerificationStatus.Verified, VerificationStatus.Verified)]
    [InlineData(VerificationStatus.None, VerificationStatus.None)]
    public void EnumUtils_ConfirmPreStatus_BehavesCorrectly(VerificationStatus current, VerificationStatus expected)
    {
        // Act
        VerificationStatus actual = EnumUtils.ConfirmPreStatus(current);

        // Assert
        Assert.Equal(expected, actual);
    }
}
